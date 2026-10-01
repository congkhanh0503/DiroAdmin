<template>
  <div 
    v-if="isOpen" 
    class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-xs animate-fade-in"
  >
    <div class="bg-slate-900 border border-slate-800 rounded-3xl max-w-2xl w-full p-6 space-y-4 shadow-2xl text-slate-200 animate-scale-up max-h-[85vh] flex flex-col">
      <!-- Modal Header -->
      <div class="flex items-center justify-between border-b border-slate-800 pb-3.5 shrink-0">
        <div class="flex items-center gap-2.5">
          <div class="w-8 h-8 rounded-xl bg-blue-600/20 border border-blue-500/30 flex items-center justify-center text-blue-400">
            <History class="w-4 h-4" />
          </div>
          <div>
            <h3 class="font-extrabold text-white text-sm">
              {{ customer ? `Lịch Sử Gia Hạn: ${customer.shopName}` : 'Lịch Sử Toàn Bộ Giao Dịch Gia Hạn' }}
            </h3>
            <p class="text-[10px] text-slate-400">
              Lưu vết kiểm toán & đối soát doanh thu bản quyền phần mềm
            </p>
          </div>
        </div>
        <button @click="$emit('close')" class="text-slate-500 hover:text-white p-1 cursor-pointer">✕</button>
      </div>

      <!-- Quick Metrics Summary -->
      <div class="grid grid-cols-2 sm:grid-cols-3 gap-3 shrink-0 text-xs">
        <div class="p-3 rounded-2xl bg-slate-950 border border-slate-800">
          <span class="text-slate-400">Số lượt gia hạn:</span>
          <p class="text-lg font-black text-white mt-0.5">{{ records.length }} lần</p>
        </div>
        <div class="p-3 rounded-2xl bg-slate-950 border border-slate-800">
          <span class="text-slate-400">Tổng tiền đã thu:</span>
          <p class="text-lg font-black text-emerald-400 mt-0.5">{{ formatCurrency(totalRevenue) }}</p>
        </div>
        <div class="p-3 rounded-2xl bg-slate-950 border border-slate-800 hidden sm:block">
          <span class="text-slate-400">Giao dịch gần nhất:</span>
          <p class="text-xs font-bold text-slate-300 mt-1 truncate">{{ latestDate }}</p>
        </div>
      </div>

      <!-- Records List Table -->
      <div class="flex-1 overflow-y-auto min-h-48 border border-slate-800/80 rounded-2xl bg-slate-950/60">
        <div v-if="records.length === 0" class="py-12 text-center text-slate-500 text-xs">
          <History class="w-8 h-8 mx-auto mb-2 text-slate-600 stroke-1" />
          <p class="font-medium text-slate-400">Chưa có bản ghi gia hạn nào</p>
          <p class="text-[11px] text-slate-500 mt-0.5">Khi bạn bấm gia hạn bất kỳ gói nào, lịch sử sẽ tự động xuất hiện tại đây.</p>
        </div>

        <table v-else class="w-full text-left text-xs text-slate-300">
          <thead class="bg-slate-900 border-b border-slate-800 text-[10px] font-bold text-slate-400 uppercase tracking-wider sticky top-0">
            <tr>
              <th class="py-3 px-3.5">Thời Gian</th>
              <th v-if="!customer" class="py-3 px-3.5">Quán</th>
              <th class="py-3 px-3.5">Gói Cấp</th>
              <th class="py-3 px-3.5">Số Tháng</th>
              <th class="py-3 px-3.5">Số Tiền Thu</th>
              <th class="py-3 px-3.5">Hạn Mới</th>
              <th class="py-3 px-3.5 text-right">Người Cấp</th>
              <th class="py-3 px-3 text-center">Xóa</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-800/60 font-medium">
            <tr 
              v-for="r in records" 
              :key="r.id || r.issuedAt"
              class="hover:bg-slate-800/30 transition text-[11px]"
            >
              <td class="py-3 px-3.5 whitespace-nowrap text-slate-400 font-mono">
                {{ formatDateTime(r.issuedAt) }}
              </td>
              <td v-if="!customer" class="py-3 px-3.5 whitespace-nowrap">
                <span class="font-bold text-white">{{ r.shopName }}</span>
                <span class="text-slate-500 font-mono ml-1 text-[10px]">({{ r.shopCode }})</span>
              </td>
              <td class="py-3 px-3.5 whitespace-nowrap">
                <span 
                  class="px-2 py-0.5 rounded text-[10px] font-extrabold uppercase border"
                  :class="getPlanBadgeClass(r.planType)"
                >
                  {{ r.planType }}
                </span>
              </td>
              <td class="py-3 px-3.5 whitespace-nowrap font-bold text-slate-200">
                +{{ r.months }} tháng
              </td>
              <td class="py-3 px-3.5 whitespace-nowrap font-black text-emerald-400">
                {{ formatCurrency(r.price) }}
              </td>
              <td class="py-3 px-3.5 whitespace-nowrap text-slate-300 font-bold">
                {{ formatDate(r.expiresAt) }}
              </td>
              <td class="py-3 px-3.5 text-right whitespace-nowrap text-slate-400 text-[10px]">
                {{ r.createdBy || 'Admin' }}
              </td>
              <td class="py-3 px-3 text-center whitespace-nowrap">
                <button
                  @click="confirmDeleteRecord(r)"
                  class="p-1 rounded-lg text-slate-500 hover:text-rose-400 hover:bg-rose-500/10 transition cursor-pointer"
                  title="Xóa phiếu gia hạn này"
                >
                  <Trash2 class="w-3.5 h-3.5" />
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Modal Footer -->
      <div class="flex items-center justify-between pt-2 border-t border-slate-800 shrink-0">
        <div class="flex items-center gap-2">
          <button
            v-if="records.length > 0"
            @click="confirmClearAll"
            class="px-3 py-1.5 rounded-xl border border-rose-500/30 bg-rose-500/10 hover:bg-rose-500/20 text-rose-300 font-bold text-[11px] transition flex items-center gap-1.5 cursor-pointer"
            title="Xóa toàn bộ lịch sử để đưa doanh thu về 0đ"
          >
            <Trash2 class="w-3.5 h-3.5" />
            <span>Xóa Sạch Lịch Sử Test</span>
          </button>
          <span class="text-[10px] text-slate-500 hidden sm:inline">
            (Xóa phiếu gia hạn sẽ tự động trừ doanh thu Dashboard)
          </span>
        </div>
        <button
          @click="$emit('close')"
          class="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-200 font-bold text-xs cursor-pointer"
        >
          Đóng
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { History, Trash2 } from 'lucide-vue-next'

const props = defineProps({
  isOpen: Boolean,
  customer: {
    type: Object,
    default: null
  },
  records: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['close', 'delete-record', 'clear-all-records'])

function confirmDeleteRecord(record) {
  const shopName = record.shopName || 'quán'
  const priceStr = formatCurrency(record.price)
  if (confirm(`Bạn có chắc chắn muốn xóa bản ghi gia hạn của "${shopName}" (${priceStr})?\n\nThao tác này sẽ tự động trừ số tiền này khỏi tổng doanh thu Dashboard.`)) {
    emit('delete-record', record)
  }
}

function confirmClearAll() {
  if (confirm('⚠️ BẠN CÓ CHẮC CHẮN MUỐN XÓA TOÀN BỘ LỊCH SỬ GIA HẠN?\n\nToàn bộ dữ liệu doanh thu test sẽ được đặt lại về 0 ₫. Thao tác này không thể hoàn tác!')) {
    emit('clear-all-records')
  }
}

const totalRevenue = computed(() => {
  return props.records.reduce((sum, r) => sum + (Number(r.price) || 0), 0)
})

const latestDate = computed(() => {
  if (props.records.length === 0) return 'Chưa có'
  const first = props.records[0]
  return formatDateTime(first.issuedAt)
})

function formatCurrency(val) {
  if (!val) return '0 ₫'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

function formatDate(isoStr) {
  if (!isoStr) return '---'
  try {
    const d = new Date(isoStr)
    return `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`
  } catch {
    return isoStr
  }
}

function formatDateTime(isoStr) {
  if (!isoStr) return '---'
  try {
    const d = new Date(isoStr)
    const date = `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`
    const time = `${String(d.getHours()).padStart(2, '0')}:${String(d.getMinutes()).padStart(2, '0')}`
    return `${time} • ${date}`
  } catch {
    return isoStr
  }
}

function getPlanBadgeClass(plan) {
  switch (plan) {
    case 'Lifetime': return 'bg-purple-500/20 text-purple-300 border-purple-500/30'
    case 'Yearly': return 'bg-indigo-500/20 text-indigo-300 border-indigo-500/30'
    case 'Monthly': return 'bg-blue-500/20 text-blue-300 border-blue-500/30'
    default: return 'bg-slate-800 text-slate-400 border-slate-700'
  }
}
</script>
