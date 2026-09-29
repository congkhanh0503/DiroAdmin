import supabase from './supabase'

export const api = {
  // 1. Thống kê Dashboard
  async getStats() {
    const { data: list, error } = await supabase.from('customers').select('*')
    if (error) throw error

    const now = new Date()
    const soon = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000)

    let total = list?.length || 0
    let active = 0
    let expiringSoon = 0
    let lockedOrExpired = 0

    list?.forEach(c => {
      const exp = new Date(c.expires_at)
      if (c.status === 'Active' && exp > now) {
        active++
        if (exp <= soon) expiringSoon++
      } else {
        lockedOrExpired++
      }
    })

    return {
      data: {
        totalCustomers: total,
        activeCount: active,
        expiringSoonCount: expiringSoon,
        lockedOrExpiredCount: lockedOrExpired,
        totalRevenue: active * 1500000 // Tạm tính doanh thu ước lượng
      }
    }
  },

  // 2. Lấy danh sách quán khách hàng
  async getCustomers(filters = {}) {
    let query = supabase.from('customers').select('*').order('created_at', { ascending: false })

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

    const payload = {
      shop_code: shopCode,
      shop_name: formData.shopName.trim(),
      owner_name: formData.ownerName?.trim() || 'Chủ tiệm',
      phone: formData.phone.trim(),
      address: formData.address?.trim() || '',
      business_model: formData.businessModel || 'Barber',
      current_plan: months >= 12 ? 'Yearly' : (months === 1 ? 'Trial' : 'Monthly'),
      activated_at: new Date().toISOString(),
      expires_at: expiresAt.toISOString(),
      status: 'Active',
      hardware_id: formData.hardwareId?.trim() || null,
      notes: formData.notes || 'Tạo từ DiroAdmin Cloud'
    }

    const { data, error } = await supabase.from('customers').insert([payload]).select().single()
    if (error) throw error

    return {
      data: {
        id: data.id,
        shopCode: data.shop_code,
        shopName: data.shop_name
      }
    }
  },

  // 4. Gia hạn tự động nhanh (+tháng) đến máy POS qua Supabase
  async quickExtend(id, { months = 1, price = 0 }) {
    // Lấy thông tin hiện tại
    const { data: current, error: getErr } = await supabase.from('customers').select('*').eq('id', id).single()
    if (getErr) throw getErr

    const now = new Date()
    const currentExp = new Date(current.expires_at)
    const baseDate = currentExp > now ? currentExp : now

    const newExp = new Date(baseDate)
    newExp.setMonth(newExp.getMonth() + months)

    const plan = months >= 12 ? 'Yearly' : (months >= 60 ? 'Lifetime' : 'Monthly')

    const { error: updErr } = await supabase.from('customers').update({
      expires_at: newExp.toISOString(),
      status: 'Active',
      current_plan: plan
    }).eq('id', id)

    if (updErr) throw updErr

    return {
      data: {
        message: `Đã gia hạn thành công +${months} tháng cho '${current.shop_name}'! Hạn mới: ${newExp.toLocaleDateString('vi-VN')}. Lệnh đã truyền lên Cloud Supabase đến máy POS!`,
        expiresAt: newExp.toISOString(),
        status: 'Active',
        planType: plan
      }
    }
  },

  // 5. Khóa / Mở Khóa Tức Thì (Kill-switch)
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

  // 6. Xóa khách hàng
  async deleteCustomer(id) {
    const { error } = await supabase.from('customers').delete().eq('id', id)
    if (error) throw error
    return { data: { success: true } }
  },

  // 7. Đăng ký Realtime Changes
  subscribeCustomers(onChange) {
    return supabase
      .channel('customers_realtime')
      .on('postgres_changes', { event: '*', schema: 'public', table: 'customers' }, payload => {
        onChange(payload)
      })
      .subscribe()
  }
}

export default api
