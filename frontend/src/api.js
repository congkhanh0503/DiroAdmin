import { supabase, supabaseAdmin } from './supabase'

export const DEFAULT_PACKAGES = [
  { label: '1 Tháng', months: 1, plan: 'Monthly', price: 99000 },
  { label: '3 Tháng', months: 3, plan: 'Monthly', price: 280000 },
  { label: '6 Tháng', months: 6, plan: 'Monthly', price: 500000 },
  { label: '1 Năm', months: 12, plan: 'Yearly', price: 990000 },
  { label: '2 Năm', months: 24, plan: 'Yearly', price: 1800000 },
  { label: 'Trọn Đời', months: 120, plan: 'Lifetime', price: 2000000 }
]

export function calculateDefaultPrice(months) {
  if (months === 1) return 99000
  if (months === 3) return 280000
  if (months === 6) return 500000
  if (months >= 12 && months < 24) return 990000
  if (months >= 24 && months < 60) return 1800000
  if (months >= 60) return 2000000
  return months * 99000
}

// Chuẩn hóa timestamp UTC để so sánh chuẩn xác bất kể có 'Z' hay không
export function parseUtc(val) {
  if (!val) return 0
  let s = String(val).trim()
  if (!s.endsWith('Z') && !/[+-]\d{2}:\d{2}$/.test(s)) {
    s += 'Z'
  }
  const t = new Date(s).getTime()
  return isNaN(t) ? 0 : t
}

// Quản lý lưu trữ lịch sử gia hạn cục bộ an toàn & bền vững
function getLocalLicenseRecords() {
  try {
    const raw = localStorage.getItem('diroadmin_license_records')
    const list = raw ? JSON.parse(raw) : []
    // Tự động khử trùng lặp trong chính LocalStorage
    const clean = []
    list.forEach(item => {
      const itemTime = parseUtc(item.issuedAt)
      const isDup = clean.some(c => {
        const cTime = parseUtc(c.issuedAt)
        const sameShop = (c.shopCode && item.shopCode && c.shopCode === item.shopCode) ||
          (c.customerId && item.customerId && c.customerId === item.customerId)
        return sameShop && Math.abs(cTime - itemTime) < 60000
      })
      if (!isDup) clean.push(item)
    })
    if (clean.length !== list.length) {
      localStorage.setItem('diroadmin_license_records', JSON.stringify(clean))
    }
    return clean
  } catch {
    return []
  }
}

function saveLocalLicenseRecord(record) {
  try {
    const list = getLocalLicenseRecords()
    // Tránh thêm nếu đã tồn tại bản ghi tương tự
    const recTime = parseUtc(record.issuedAt)
    const exists = list.some(r => {
      const rTime = parseUtc(r.issuedAt)
      const sameShop = (r.shopCode && record.shopCode && r.shopCode === record.shopCode) ||
        (r.customerId && record.customerId && r.customerId === record.customerId)
      return sameShop && Math.abs(rTime - recTime) < 60000
    })
    if (!exists) {
      list.unshift(record)
      localStorage.setItem('diroadmin_license_records', JSON.stringify(list))
    }
  } catch (err) {
    console.warn('Lỗi lưu lịch sử local:', err)
  }
}

export const api = {
  // 1. Thống kê Dashboard & Doanh Thu Thực Tế
  async getStats() {
    const { data: list, error } = await supabase.from('customers').select('*').neq('status', 'Deleted')
    if (error) throw error

    const now = new Date()
    const soon = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000)

    const validList = list || []
    let total = validList.length
    let active = 0
    let expiringSoon = 0
    let lockedOrExpired = 0

    validList.forEach(c => {
      const exp = new Date(c.expires_at)
      if (c.status === 'Active' && exp > now) {
        active++
        if (exp <= soon) expiringSoon++
      } else {
        lockedOrExpired++
      }
    })

    // Tính tổng doanh thu thực tế từ tất cả các lần gia hạn đã lưu
    const records = await this.getLicenseRecords()
    const totalRevenue = records.reduce((sum, r) => sum + (Number(r.price) || 0), 0)

    return {
      data: {
        totalCustomers: total,
        activeCount: active,
        expiringSoonCount: expiringSoon,
        lockedOrExpiredCount: lockedOrExpired,
        totalRevenue: totalRevenue > 0 ? totalRevenue : (active * 99000) // Fallback tối thiểu
      }
    }
  },

  // 2. Lấy danh sách quán khách hàng
  async getCustomers(filters = {}) {
    let query = supabase.from('customers').select('*').neq('status', 'Deleted').order('created_at', { ascending: false })

    if (filters.search) {
      const s = filters.search.trim()
      query = query.or(`shop_name.ilike.%${s}%,shop_code.ilike.%${s}%,owner_name.ilike.%${s}%,phone.ilike.%${s}%`)
    }

    if (filters.status) {
      query = query.eq('status', filters.status)
    }

    if (filters.plan) {
      query = query.eq('current_plan', filters.plan)
    }

    const { data, error } = await query
    if (error) throw error

    // Chuyển snake_case sang camelCase cho Vue UI
    const mapped = (data || []).map(c => ({
      id: c.id,
      shopCode: c.shop_code,
      shopName: c.shop_name,
      ownerName: c.owner_name,
      phone: c.phone,
      address: c.address,
      businessModel: c.business_model,
      currentPlan: c.current_plan,
      activatedAt: c.activated_at,
      expiresAt: c.expires_at,
      status: c.status,
      hardwareId: c.hardware_id,
      activeLicenseKey: c.active_license_key,
      notes: c.notes,
      lastPingAt: c.last_ping_at,
      createdAt: c.created_at
    }))

    return { data: mapped }
  },

  // 3. Thêm mới Quán Khách Hàng
  async createCustomer(formData) {
    const shopCode = formData.shopCode?.trim().toUpperCase() || `DP-${Math.floor(1000 + Math.random() * 9000)}`
    const months = formData.initialMonths || 1
    const expiresAt = new Date()
    expiresAt.setMonth(expiresAt.getMonth() + months)

    const initialPrice = formData.price > 0 ? formData.price : calculateDefaultPrice(months)
    const plan = months >= 60 ? 'Lifetime' : (months >= 12 ? 'Yearly' : (months === 1 ? 'Trial' : 'Monthly'))

    const payload = {
      shop_code: shopCode,
      shop_name: formData.shopName.trim(),
      owner_name: formData.ownerName?.trim() || 'Chủ tiệm',
      phone: formData.phone.trim(),
      address: formData.address?.trim() || '',
      business_model: formData.businessModel || 'Barber',
      current_plan: plan,
      activated_at: new Date().toISOString(),
      expires_at: expiresAt.toISOString(),
      status: 'Active',
      hardware_id: formData.hardwareId?.trim() || null,
      notes: formData.notes || 'Tạo từ DiroAdmin Cloud'
    }

    const { data, error } = await supabase.from('customers').insert([payload]).select().single()
    if (error) throw error

    // Lưu ngay bản ghi cấp bản quyền đầu tiên vào lịch sử gia hạn
    const initialRecord = {
      id: Date.now(),
      customerId: data.id,
      shopCode: data.shop_code,
      shopName: data.shop_name,
      planType: plan,
      months: months,
      price: initialPrice,
      issuedAt: new Date().toISOString(),
      expiresAt: expiresAt.toISOString(),
      createdBy: 'Admin (Khởi Tạo)'
    }
    saveLocalLicenseRecord(initialRecord)

    // Đồng bộ vào backend SQLite nếu có
    try {
      fetch('http://localhost:5020/api/customers', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          shopCode: data.shop_code,
          shopName: data.shop_name,
          ownerName: data.owner_name,
          phone: data.phone,
          address: data.address,
          businessModel: data.business_model,
          planType: plan,
          initialMonths: months,
          price: initialPrice,
          hardwareId: data.hardware_id
        })
      }).catch(() => { })
    } catch { }

    return {
      data: {
        id: data.id,
        shopCode: data.shop_code,
        shopName: data.shop_name
      }
    }
  },

  // 4. Gia hạn tự động nhanh (+tháng) đến máy POS qua Supabase & Ghi Lịch Sử Doanh Thu
  async quickExtend(id, { months = 1, price = 0 }) {
    // 1. Lấy thông tin hiện tại của quán
    const { data: current, error: getErr } = await supabase.from('customers').select('*').eq('id', id).single()
    if (getErr) throw getErr

    const now = new Date()
    const currentExp = new Date(current.expires_at)
    const baseDate = currentExp > now ? currentExp : now

    const newExp = new Date(baseDate)
    newExp.setMonth(newExp.getMonth() + months)

    // Xác định tên gói cước
    const plan = months >= 60 ? 'Lifetime' : (months >= 12 ? 'Yearly' : 'Monthly')
    // Xác định số tiền thu (nếu không nhập thì tính theo giá chuẩn 99k/tháng, 2m trọn đời)
    const finalPrice = price > 0 ? Number(price) : calculateDefaultPrice(months)

    // 2. Cập nhật hạn dùng lên Supabase (máy POS sẽ tự nhận ngay)
    const { error: updErr } = await supabase.from('customers').update({
      expires_at: newExp.toISOString(),
      status: 'Active',
      current_plan: plan
    }).eq('id', id)

    if (updErr) throw updErr

    // 3. Ghi vào lịch sử gia hạn (Lưu vết doanh thu & kiểm toán)
    const newRecord = {
      id: Date.now(),
      customerId: current.id,
      shopCode: current.shop_code,
      shopName: current.shop_name,
      planType: plan,
      months: months,
      price: finalPrice,
      issuedAt: new Date().toISOString(),
      expiresAt: newExp.toISOString(),
      createdBy: 'Admin (Gia Hạn)'
    }

    // Lưu vào LocalStorage
    saveLocalLicenseRecord(newRecord)

    // Thử lưu vào Supabase table license_records nếu table đã được tạo
    try {
      await supabase.from('license_records').insert([{
        customer_id: current.id,
        shop_code: current.shop_code,
        shop_name: current.shop_name,
        plan_type: plan,
        months: months,
        price: finalPrice,
        issued_at: newRecord.issuedAt,
        expires_at: newExp.toISOString(),
        created_by: 'Admin (Gia Hạn)'
      }])
    } catch { }

    // Đồng bộ sang Backend SQLite DiroAdmin API /license-records
    try {
      fetch('http://localhost:5020/api/customers/license-records', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          shopCode: current.shop_code,
          shopName: current.shop_name,
          customerId: current.id,
          planType: plan,
          months: months,
          price: finalPrice,
          issuedAt: newRecord.issuedAt,
          expiresAt: newExp.toISOString(),
          createdBy: 'Admin (Gia Hạn)'
        })
      }).catch(() => { })
    } catch { }

    return {
      data: {
        message: `Đã gia hạn thành công +${months} tháng cho '${current.shop_name}'! Hạn mới: ${newExp.toLocaleDateString('vi-VN')}. Doanh thu ghi nhận: ${new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(finalPrice)}`,
        expiresAt: newExp.toISOString(),
        status: 'Active',
        planType: plan,
        price: finalPrice,
        newRecord
      }
    }
  },

  // 5. Lấy toàn bộ lịch sử gia hạn (hoặc theo từng quán) - Hợp nhất Local + Backend + Supabase
  async getLicenseRecords(customerId = null, shopCode = null) {
    let supabaseRecords = []
    let backendRecords = []
    const localRecords = getLocalLicenseRecords()

    // 1. Thử lấy từ Supabase nếu có bảng
    try {
      let query = supabase.from('license_records').select('*').order('issued_at', { ascending: false })
      if (customerId) query = query.eq('customer_id', customerId)
      if (shopCode) query = query.eq('shop_code', shopCode)
      const { data, error } = await query
      if (!error && data && data.length > 0) {
        supabaseRecords = data.map(r => ({
          id: r.id,
          customerId: r.customer_id,
          shopCode: r.shop_code,
          shopName: r.shop_name,
          planType: r.plan_type,
          months: r.months,
          price: Number(r.price) || 0,
          issuedAt: r.issued_at,
          expiresAt: r.expires_at,
          createdBy: r.created_by
        }))
      }
    } catch { }

    // 2. Thử lấy từ Backend API
    try {
      const url = 'http://localhost:5020/api/customers/license-records'
      const res = await fetch(url)
      if (res.ok) {
        const raw = await res.json()
        if (Array.isArray(raw)) {
          backendRecords = raw.map(r => ({
            id: r.id,
            customerId: r.customerId,
            shopCode: r.shopCode,
            shopName: r.shopName,
            planType: r.planType,
            months: r.months,
            price: Number(r.price) || 0,
            issuedAt: r.issuedAt,
            expiresAt: r.expiresAt,
            createdBy: r.createdBy
          }))
        }
      }
    } catch (err) {
      console.warn('Backend API license-records không khả dụng:', err)
    }

    // 3. Hợp nhất (Merge) & Khử trùng lặp (Deduplicate)
    // Lấy Backend làm nguồn chuẩn, bổ sung từ Supabase và LocalStorage (nếu có bản ghi offline)
    const merged = [...backendRecords]

    const addUnique = (item) => {
      const itemTime = parseUtc(item.issuedAt)
      const isDuplicate = merged.some(m => {
        const mTime = parseUtc(m.issuedAt)
        const sameShop = (m.shopCode && item.shopCode && m.shopCode === item.shopCode) ||
          (m.customerId && item.customerId && m.customerId === item.customerId)
        return sameShop && Math.abs(mTime - itemTime) < 60000 // trong vòng 60 giây cùng shop là cùng 1 giao dịch
      })
      if (!isDuplicate) {
        merged.push(item)
      }
    }

    supabaseRecords.forEach(addUnique)
    localRecords.forEach(addUnique)

    // Sắp xếp thời gian giảm dần (mới nhất lên trên)
    merged.sort((a, b) => parseUtc(b.issuedAt) - parseUtc(a.issuedAt))

    // 4. Nếu có filter theo khách hàng cụ thể
    if (customerId || shopCode) {
      return merged.filter(r => {
        if (customerId && (r.customerId === customerId || r.customer_id === customerId)) return true
        if (shopCode && r.shopCode === shopCode) return true
        return false
      })
    }

    return merged
  },

  // 6. Khóa / Mở Khóa Tức Thì (Kill-switch)
  async toggleLock(id) {
    const { data: current, error: getErr } = await supabase.from('customers').select('status, shop_name').eq('id', id).single()
    if (getErr) throw getErr

    const newStatus = current.status === 'Suspended' ? 'Active' : 'Suspended'
    const { error: updErr } = await supabase.from('customers').update({ status: newStatus }).eq('id', id)
    if (updErr) throw updErr

    return {
      data: {
        status: newStatus,
        message: newStatus === 'Suspended' ? `Đã khóa quán '${current.shop_name}' thành công.` : `Đã mở khóa quán '${current.shop_name}' thành công.`
      }
    }
  },

  // 7. Xóa khách hàng
  async deleteCustomer(id) {
    try {
      const { data, error } = await supabase.from('customers').delete().eq('id', id).select()
      if (!error && data && data.length > 0) {
        return { data: { success: true } }
      }
    } catch { }

    const { error: updErr } = await supabase.from('customers').update({
      status: 'Deleted',
      hardware_id: null,
      notes: 'Đã xóa bởi Admin lúc ' + new Date().toLocaleString('vi-VN')
    }).eq('id', id)

    if (updErr) throw updErr
    return { data: { success: true } }
  },

  // 8. Cập nhật thông tin quán (Edit/Update)
  async updateCustomer(id, formData) {
    const payload = {}
    if (formData.shopName !== undefined) payload.shop_name = formData.shopName.trim()
    if (formData.ownerName !== undefined) payload.owner_name = formData.ownerName?.trim() || 'Chủ tiệm'
    if (formData.phone !== undefined) payload.phone = formData.phone?.trim() || ''
    if (formData.address !== undefined) payload.address = formData.address?.trim() || ''
    if (formData.businessModel !== undefined) payload.business_model = formData.businessModel
    if (formData.currentPlan !== undefined) payload.current_plan = formData.currentPlan
    if (formData.status !== undefined) payload.status = formData.status
    if (formData.notes !== undefined) payload.notes = formData.notes || ''
    if (formData.hardwareId !== undefined) payload.hardware_id = formData.hardwareId?.trim() || null
    if (formData.expiresAt) {
      payload.expires_at = new Date(formData.expiresAt).toISOString()
    }

    const { data, error } = await supabase
      .from('customers')
      .update(payload)
      .eq('id', id)
      .select()
      .single()

    if (error) throw error

    return {
      data: {
        id: data.id,
        shopCode: data.shop_code,
        shopName: data.shop_name
      }
    }
  },

  // 9. Đăng ký Realtime Changes
  subscribeCustomers(onChange) {
    return supabase
      .channel('customers_realtime')
      .on('postgres_changes', { event: '*', schema: 'public', table: 'customers' }, payload => {
        onChange(payload)
      })
      .subscribe()
  },

  // 10. Lấy danh sách 3 bản sao lưu Cloud của quán từ Supabase Storage
  async getCustomerBackups(shopCode) {
    if (!shopCode) return []
    try {
      const { data, error } = await supabaseAdmin.storage.from('pos-backups').list(shopCode.trim(), {
        limit: 10,
        sortBy: { column: 'created_at', order: 'desc' }
      })
      if (error) throw error

      return (data || [])
        .filter(f => f.name && f.name.endsWith('.zip'))
        .slice(0, 3)
        .map(f => {
          const { data: pub } = supabaseAdmin.storage.from('pos-backups').getPublicUrl(`${shopCode.trim()}/${f.name}`)
          const sizeKb = Math.round((f.metadata?.size || 0) / 1024 * 10) / 10
          return {
            name: f.name,
            size: f.metadata?.size || 0,
            sizeFormatted: `${sizeKb} KB`,
            createdAt: f.created_at,
            downloadUrl: pub.publicUrl
          }
        })
    } catch (err) {
      console.warn('Lỗi lấy danh sách backup Supabase Storage:', err)
      return []
    }
  },

  // 11. Quản Lý Phiên Bản DiroPos (App Versions Release)
  async getAppVersions() {
    try {
      const { data, error } = await supabase
        .from('app_versions')
        .select('*')
        .order('release_date', { ascending: false })
      if (error) throw error
      return data || []
    } catch (err) {
      console.warn('Lỗi lấy danh sách phiên bản từ Supabase:', err)
      return []
    }
  },

  async createAppVersion(payload) {
    const { data, error } = await supabase
      .from('app_versions')
      .insert([{
        version: payload.version.trim(),
        release_date: payload.release_date || new Date().toISOString(),
        changelog: payload.changelog?.trim() || '',
        download_url: payload.download_url?.trim() || '',
        is_mandatory: Boolean(payload.is_mandatory),
        min_version: payload.min_version?.trim() || '1.0.0'
      }])
      .select()
    if (error) throw error
    return data?.[0]
  },

  async deleteAppVersion(id) {
    const { error } = await supabase
      .from('app_versions')
      .delete()
      .eq('id', id)
    if (error) throw error
    return true
  }
}

export default api
