<template>
  <div class="space-y-5 animate-fade-in">
    <!-- Top Filter & Search Bar -->
    <div class="bg-slate-900 border border-slate-800 p-4 sm:p-5 rounded-3xl space-y-4 shadow-sm">
      <div class="flex flex-col sm:flex-row items-center justify-between gap-3">
        <!-- Search Input -->
        <div class="relative w-full sm:w-96">
          <input
            :value="searchQuery"
            @input="$emit('update:searchQuery', $event.target.value)"
            type="text"
            placeholder="Tìm theo tên tiệm, chủ quán, SĐT, mã quán, Hardware ID..."
            class="w-full bg-slate-950 border border-slate-800 rounded-2xl px-4 py-2.5 pl-10 text-xs text-slate-200 placeholder-slate-500 focus:outline-none focus:border-indigo-500 focus:ring-1 focus:ring-indigo-500/30 transition"
          />
          <Search class="w-4 h-4 text-slate-500 absolute left-3.5 top-3" />
        </div>

        <!-- View Mode Switcher & Quick Add Button -->
        <div class="flex items-center gap-2.5 w-full sm:w-auto justify-between sm:justify-end">
          <!-- Chuyển đổi View: Table vs Grid Cards -->
          <div class="flex items-center bg-slate-950 p-1 rounded-xl border border-slate-800">
            <button
              @click="viewMode = 'table'"
              class="px-3 py-1.5 rounded-lg text-xs font-semibold flex items-center gap-1.5 transition cursor-pointer"
              :class="viewMode === 'table' ? 'bg-indigo-600 text-white shadow-xs' : 'text-slate-400 hover:text-slate-200'"
              title="Xem dạng Bảng chi tiết"
            >
              <LayoutList class="w-3.5 h-3.5" />
              <span class="hidden sm:inline">Dạng Bảng</span>
            </button>
            <button
              @click="viewMode = 'grid'"
              class="px-3 py-1.5 rounded-lg text-xs font-semibold flex items-center gap-1.5 transition cursor-pointer"
              :class="viewMode === 'grid' ? 'bg-indigo-600 text-white shadow-xs' : 'text-slate-400 hover:text-slate-200'"
              title="Xem dạng Thẻ trực quan"
            >
              <LayoutGrid class="w-3.5 h-3.5" />
              <span class="hidden sm:inline">Dạng Thẻ</span>
            </button>
          </div>

          <button
            @click="$emit('open-new-customer')"
            class="px-4 py-2 rounded-xl bg-gradient-to-r from-indigo-600 to-blue-600 hover:from-indigo-500 hover:to-blue-500 text-white font-bold text-xs shadow-md shadow-indigo-600/30 transition flex items-center gap-1.5 cursor-pointer whitespace-nowrap active:scale-95"
          >
            <Plus class="w-4 h-4" />
            <span>Thêm Quán</span>
          </button>
        </div>
      </div>

      <!-- Filter Row -->
      <div class="flex flex-wrap items-center gap-2.5 pt-3 border-t border-slate-800/80 text-xs">
        <span class="text-slate-400 font-medium flex items-center gap-1">
          <Filter class="w-3.5 h-3.5" />
          <span>Bộ lọc:</span>
        </span>

        <!-- Lọc Trạng Thái -->
        <select
          :value="filterStatus"
          @change="$emit('update:filterStatus', $event.target.value)"
          class="bg-slate-950 border border-slate-800 rounded-xl px-3 py-1.5 text-slate-300 focus:outline-none focus:border-indigo-500 cursor-pointer"
        >
          <option value="">Tất cả trạng thái</option>
          <option value="Active">Đang hoạt động</option>
          <option value="ExpiringSoon">Sắp hết hạn (7 ngày)</option>
          <option value="Expired">Đã hết hạn</option>
          <option value="Suspended">Đã tạm khóa</option>
        </select>

        <!-- Lọc Gói Cước -->
        <select
          :value="filterPlan"
          @change="$emit('update:filterPlan', $event.target.value)"
          class="bg-slate-950 border border-slate-800 rounded-xl px-3 py-1.5 text-slate-300 focus:outline-none focus:border-indigo-500 cursor-pointer"
        >
          <option value="">Tất cả gói cước</option>
          <option value="Trial">Dùng thử (Trial)</option>
          <option value="Monthly">Gói Tháng (Monthly)</option>
          <option value="Yearly">Gói Năm (Yearly)</option>
          <option value="Lifetime">Trọn Đời (Lifetime)</option>
        </select>

        <!-- Lọc Mô hình kinh doanh -->
        <select
          :value="filterModel"
          @change="$emit('update:filterModel', $event.target.value)"
          class="bg-slate-950 border border-slate-800 rounded-xl px-3 py-1.5 text-slate-300 focus:outline-none focus:border-indigo-500 cursor-pointer"
        >
          <option value="">Tất cả mô hình</option>
          <option value="Barber">Barber (Tóc Nam)</option>
          <option value="Salon">Salon Tóc Nữ</option>
          <option value="Spa">Nail & Spa Thẩm Mỹ</option>
          <option value="Retail">Cửa Hàng Bán Lẻ</option>
          <option value="Cafe">Quán Cafe / Khác</option>
        </select>

        <!-- Reset filter button -->
        <button
          v-if="searchQuery || filterStatus || filterPlan || filterModel"
          @click="$emit('reset-filters')"
          class="px-2.5 py-1.5 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 transition text-[11px] font-semibold"
        >
          Xóa lọc ✕
        </button>

        <span class="ml-auto text-slate-400 font-mono text-[11px]">
          Hiển thị <b class="text-white">{{ filteredCustomers.length }}</b> / {{ customers.length }} tiệm
        </span>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="filteredCustomers.length === 0" class="py-16 text-center text-slate-500 bg-slate-900 border border-slate-800 rounded-3xl">
      <Store class="w-12 h-12 mx-auto mb-3 text-slate-600 stroke-1" />
      <p class="text-sm font-semibold text-slate-300">Không tìm thấy quán khách hàng nào</p>
      <p class="text-xs text-slate-500 mt-1 max-w-sm mx-auto">Vui lòng thử điều chỉnh lại bộ lọc hoặc từ khóa tìm kiếm của bạn.</p>
    </div>

    <!-- MODE 1: DẠNG BẢNG (TABLE VIEW) -->
    <div v-else-if="viewMode === 'table'" class="bg-slate-900 border border-slate-800 rounded-3xl overflow-hidden shadow-sm">
      <div class="overflow-x-auto">
        <table class="w-full text-left text-xs text-slate-300">
          <thead class="bg-slate-950/70 border-b border-slate-800 text-[11px] font-bold text-slate-400 uppercase tracking-wider">
            <tr>
              <th class="py-4 px-4">Mã Quán</th>
              <th class="py-4 px-4">Tiệm & Thiết Bị POS</th>
              <th class="py-4 px-4">Chủ Tiệm & SĐT</th>
              <th class="py-4 px-4">Mô Hình</th>
              <th class="py-4 px-4">Gói Cước</th>
              <th class="py-4 px-4">Hạn Sử Dụng</th>
              <th class="py-4 px-4">Trạng Thái</th>
              <th class="py-4 px-4 text-right">Gia Hạn Nhanh & Thao Tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-800/60">
            <tr
              v-for="c in filteredCustomers"
              :key="c.id"
              class="hover:bg-slate-800/40 transition group"
            >
              <!-- Mã Quán -->
              <td class="py-4 px-4 whitespace-nowrap">
                <span class="font-mono font-bold text-indigo-400 bg-indigo-500/10 border border-indigo-500/20 px-2 py-1 rounded-lg">
                  {{ c.shopCode }}
                </span>
              </td>

              <!-- Tiệm & Thiết Bị POS -->
              <td class="py-4 px-4">
                <div class="flex items-center gap-2">
                  <p class="font-bold text-white text-xs">{{ c.shopName }}</p>
                  <!-- Trạng thái Online POS -->
                  <span
                    class="inline-flex items-center gap-1 px-1.5 py-0.5 rounded text-[9px] font-bold"
                    :class="isOnline(c) ? 'bg-emerald-500/20 text-emerald-400 border border-emerald-500/30' : 'bg-slate-800 text-slate-400 border border-slate-700'"
                  >
                    <span class="w-1.5 h-1.5 rounded-full" :class="isOnline(c) ? 'bg-emerald-400 animate-pulse' : 'bg-slate-500'"></span>
                    <span>{{ isOnline(c) ? 'Online POS' : 'Offline' }}</span>
                  </span>
                </div>
                <p class="text-[11px] text-slate-400 truncate max-w-xs mt-0.5">{{ c.address || 'Chưa cập nhật địa chỉ' }}</p>
                <div class="flex items-center gap-2 mt-1 text-[10px] text-slate-500 font-mono">
                  <span class="bg-slate-950 px-1.5 py-0.5 rounded border border-slate-800 text-slate-400">
                    ID: {{ c.hardwareId || 'Chưa gắn máy' }}
                  </span>
                  <span v-if="c.lastPingAt" class="text-[9px]">
                    Ping: {{ formatLastPing(c) }}
                  </span>
                </div>
              </td>

              <!-- Chủ Quán & SĐT -->
              <td class="py-4 px-4 whitespace-nowrap">
                <p class="font-medium text-slate-200">{{ c.ownerName }}</p>
                <a :href="'tel:' + c.phone" class="text-[11px] font-mono text-emerald-400 hover:underline flex items-center gap-1 mt-0.5">
                  <Phone class="w-3 h-3" />
                  <span>{{ c.phone }}</span>
                </a>
              </td>

              <!-- Mô Hình -->
              <td class="py-4 px-4 whitespace-nowrap">
                <span class="px-2 py-0.5 rounded-lg text-[10px] font-medium bg-slate-950 border border-slate-800 text-slate-300">
                  {{ formatBusinessModel(c.businessModel) }}
                </span>
              </td>

              <!-- Gói Cước -->
              <td class="py-4 px-4 whitespace-nowrap">
                <span
                  class="px-2 py-0.5 rounded-lg text-[10px] font-extrabold uppercase border"
                  :class="getPlanBadgeClass(c.currentPlan)"
                >
                  {{ c.currentPlan }}
                </span>
              </td>

              <!-- Hạn Dùng & Số ngày còn -->
              <td class="py-4 px-4 whitespace-nowrap">
                <p class="font-bold text-slate-200">{{ formatDate(c.expiresAt) }}</p>
                <p class="text-[10px]" :class="getDaysRemainingClass(c)">
                  {{ getDaysRemainingText(c) }}
                </p>
              </td>

              <!-- Trạng Thái -->
              <td class="py-4 px-4 whitespace-nowrap">
                <span
                  class="px-2.5 py-1 rounded-full text-[10px] font-extrabold flex items-center gap-1.5 w-max border"
                  :class="getStatusBadgeClass(c)"
                >
                  <span class="w-1.5 h-1.5 rounded-full" :class="getStatusDotClass(c)"></span>
                  <span>{{ getStatusText(c) }}</span>
                </span>
              </td>

              <!-- Thao Tác & Gia Hạn Nhanh -->
              <td class="py-4 px-4 text-right whitespace-nowrap">
                <div class="flex items-center justify-end gap-1.5">
                  <!-- Nút Gia Hạn Nhanh 1 Tháng -->
                  <button
                    @click="$emit('quick-extend-1m', c)"
                    title="Gia hạn nhanh +1 Tháng đến máy POS"
                    class="px-2 py-1 rounded-lg bg-emerald-600/20 hover:bg-emerald-600 text-emerald-400 hover:text-white transition font-bold text-[11px] flex items-center gap-1 cursor-pointer border border-emerald-500/30"
                  >
                    <Zap class="w-3 h-3" />
                    <span>+1T</span>
                  </button>

                  <!-- Nút Gia Hạn Nhanh 1 Năm -->
                  <button
                    @click="$emit('quick-extend-1y', c)"
                    title="Gia hạn nhanh +1 Năm đến máy POS"
                    class="px-2 py-1 rounded-lg bg-indigo-600/20 hover:bg-indigo-600 text-indigo-400 hover:text-white transition font-bold text-[11px] flex items-center gap-1 cursor-pointer border border-indigo-500/30"
                  >
                    <Zap class="w-3 h-3" />
                    <span>+1N</span>
                  </button>

                  <!-- Nút Mở Modal Gia Hạn Tùy Chọn -->
                  <button
                    @click="$emit('open-license-modal', c)"
                    title="Tùy chọn gia hạn & xem chi tiết"
                    class="p-1.5 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-300 hover:text-white transition cursor-pointer"
                  >
                    <Key class="w-3.5 h-3.5" />
                  </button>

                  <!-- Nút Xem Lịch Sử Gia Hạn -->
                  <button
                    @click="$emit('open-history', c)"
                    title="Xem lịch sử gia hạn của tiệm này"
                    class="p-1.5 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-300 hover:text-white transition cursor-pointer"
                  >
                    <History class="w-3.5 h-3.5" />
                  </button>

                  <!-- Nút Xem Bản Sao Lưu Cloud -->
                  <button
                    @click="$emit('open-backups', c)"
                    title="Bản sao lưu Cloud (Supabase)"
                    class="p-1.5 rounded-lg bg-slate-800 hover:bg-cyan-900/40 text-cyan-400 hover:text-cyan-200 transition cursor-pointer"
                  >
                    <Cloud class="w-3.5 h-3.5" />
                  </button>

                  <!-- Nút Chỉnh Sửa Thông Tin Quán -->
                  <button
                    @click="$emit('open-edit-modal', c)"
                    title="Chỉnh sửa thông tin quán"
                    class="p-1.5 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-300 hover:text-white transition cursor-pointer"
                  >
                    <Edit3 class="w-3.5 h-3.5" />
                  </button>

                  <!-- Nút Khóa / Mở Khóa Tức Thì -->
                  <button
                    @click="$emit('toggle-lock', c)"
                    :title="c.status === 'Suspended' ? 'Mở Khóa Quán Tức Thì' : 'Khóa Quán Tức Thì (Kill-switch)'"
                    class="p-1.5 rounded-lg transition cursor-pointer"
                    :class="c.status === 'Suspended' ? 'bg-emerald-600/20 text-emerald-400 hover:bg-emerald-600 hover:text-white' : 'bg-rose-600/20 text-rose-400 hover:bg-rose-600 hover:text-white'"
                  >
                    <Lock v-if="c.status !== 'Suspended'" class="w-3.5 h-3.5" />
                    <Unlock v-else class="w-3.5 h-3.5" />
                  </button>

                  <!-- Nút Xóa -->
                  <button
                    @click="$emit('delete-customer', c)"
                    title="Xóa Quán"
                    class="p-1.5 rounded-lg bg-slate-800 hover:bg-rose-900/50 text-slate-400 hover:text-rose-300 transition cursor-pointer"
                  >
                    <Trash2 class="w-3.5 h-3.5" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- MODE 2: DẠNG LƯỚI THẺ (GRID CARDS VIEW) -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div
        v-for="c in filteredCustomers"
        :key="c.id"
        class="bg-slate-900 border border-slate-800 rounded-3xl p-5 space-y-4 hover:border-slate-700 transition flex flex-col justify-between group shadow-sm"
      >
        <!-- Card Header -->
        <div class="space-y-2">
          <div class="flex items-start justify-between gap-2">
            <div>
              <span class="font-mono text-[10px] font-bold text-indigo-400 bg-indigo-500/10 border border-indigo-500/20 px-2 py-0.5 rounded">
                {{ c.shopCode }}
              </span>
              <h4 class="font-bold text-white text-sm mt-1.5 group-hover:text-indigo-300 transition">
                {{ c.shopName }}
              </h4>
            </div>

            <!-- Trạng thái Online -->
            <span
              class="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-[9px] font-bold"
              :class="isOnline(c) ? 'bg-emerald-500/20 text-emerald-400 border border-emerald-500/30' : 'bg-slate-800 text-slate-400 border border-slate-700'"
            >
              <span class="w-1.5 h-1.5 rounded-full" :class="isOnline(c) ? 'bg-emerald-400 animate-pulse' : 'bg-slate-500'"></span>
              <span>{{ isOnline(c) ? 'POS Online' : 'Offline' }}</span>
            </span>
          </div>

          <p class="text-xs text-slate-400 flex items-center gap-1">
            <MapPin class="w-3.5 h-3.5 shrink-0 text-slate-500" />
            <span class="truncate">{{ c.address || 'Chưa cập nhật địa chỉ' }}</span>
          </p>
        </div>

        <!-- Card Body Info -->
        <div class="bg-slate-950/70 p-3 rounded-2xl border border-slate-800/80 space-y-1.5 text-xs">
          <div class="flex justify-between items-center">
            <span class="text-slate-400">Chủ tiệm:</span>
            <span class="font-semibold text-slate-200">{{ c.ownerName }}</span>
          </div>
          <div class="flex justify-between items-center">
            <span class="text-slate-400">Số điện thoại:</span>
            <a :href="'tel:' + c.phone" class="font-mono text-emerald-400 hover:underline">
              {{ c.phone }}
            </a>
          </div>
          <div class="flex justify-between items-center">
            <span class="text-slate-400">Mô hình:</span>
            <span class="text-slate-300 font-medium">{{ formatBusinessModel(c.businessModel) }}</span>
          </div>
          <div class="flex justify-between items-center pt-1 border-t border-slate-800/60">
            <span class="text-slate-400">Gói cước:</span>
            <span
              class="px-2 py-0.5 rounded text-[10px] font-extrabold uppercase border"
              :class="getPlanBadgeClass(c.currentPlan)"
            >
              {{ c.currentPlan }}
            </span>
          </div>
          <div class="flex justify-between items-center">
            <span class="text-slate-400">Hạn sử dụng:</span>
            <div class="text-right">
              <span class="font-bold text-slate-200">{{ formatDate(c.expiresAt) }}</span>
              <p class="text-[10px]" :class="getDaysRemainingClass(c)">{{ getDaysRemainingText(c) }}</p>
            </div>
          </div>
        </div>

        <!-- Card Footer Actions -->
        <div class="pt-2 border-t border-slate-800/80 flex items-center justify-between gap-1.5">
          <div class="flex items-center gap-1.5">
            <button
              @click="$emit('quick-extend-1m', c)"
              class="px-2.5 py-1.5 rounded-xl bg-emerald-600/20 hover:bg-emerald-600 text-emerald-400 hover:text-white transition font-bold text-xs flex items-center gap-1 cursor-pointer border border-emerald-500/30"
              title="Gia hạn +1 Tháng"
            >
              <Zap class="w-3.5 h-3.5" />
              <span>+1T</span>
            </button>
            <button
              @click="$emit('quick-extend-1y', c)"
              class="px-2.5 py-1.5 rounded-xl bg-indigo-600/20 hover:bg-indigo-600 text-indigo-400 hover:text-white transition font-bold text-xs flex items-center gap-1 cursor-pointer border border-indigo-500/30"
              title="Gia hạn +1 Năm"
            >
              <Zap class="w-3.5 h-3.5" />
              <span>+1N</span>
            </button>
          </div>

          <div class="flex items-center gap-1">
            <button
              @click="$emit('open-license-modal', c)"
              title="Tùy chọn gia hạn"
              class="p-1.5 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 hover:text-white transition cursor-pointer"
            >
              <Key class="w-3.5 h-3.5" />
            </button>
            <button
              @click="$emit('open-history', c)"
              title="Xem lịch sử gia hạn của quán"
              class="p-1.5 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 hover:text-white transition cursor-pointer"
            >
              <History class="w-3.5 h-3.5" />
            </button>
            <button
              @click="$emit('open-backups', c)"
              title="Bản sao lưu Cloud (Supabase)"
              class="p-1.5 rounded-xl bg-slate-800 hover:bg-cyan-900/40 text-cyan-400 hover:text-cyan-200 transition cursor-pointer"
            >
              <Cloud class="w-3.5 h-3.5" />
            </button>
            <button
              @click="$emit('open-edit-modal', c)"
              title="Sửa thông tin tiệm"
              class="p-1.5 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 hover:text-white transition cursor-pointer"
            >
              <Edit3 class="w-3.5 h-3.5" />
            </button>
            <button
              @click="$emit('toggle-lock', c)"
              :title="c.status === 'Suspended' ? 'Mở khóa' : 'Khóa quán'"
              class="p-1.5 rounded-xl transition cursor-pointer"
              :class="c.status === 'Suspended' ? 'bg-emerald-600/20 text-emerald-400 hover:bg-emerald-600 hover:text-white' : 'bg-rose-600/20 text-rose-400 hover:bg-rose-600 hover:text-white'"
            >
              <Lock v-if="c.status !== 'Suspended'" class="w-3.5 h-3.5" />
              <Unlock v-else class="w-3.5 h-3.5" />
            </button>
            <button
              @click="$emit('delete-customer', c)"
              title="Xóa tiệm"
              class="p-1.5 rounded-xl bg-slate-800 hover:bg-rose-900/50 text-slate-400 hover:text-rose-300 transition cursor-pointer"
            >
              <Trash2 class="w-3.5 h-3.5" />
            </button>
          </div>
        </div>

      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import {
  Store,
  Search,
  Filter,
  LayoutList,
  LayoutGrid,
  Plus,
  Phone,
  MapPin,
  Zap,
  Key,
  History,
  Edit3,
  Lock,
  Unlock,
  Trash2,
  Cloud
} from 'lucide-vue-next'

const props = defineProps({
  customers: {
    type: Array,
    default: () => []
  },
  searchQuery: {
    type: String,
    default: ''
  },
  filterStatus: {
    type: String,
    default: ''
  },
  filterPlan: {
    type: String,
    default: ''
  },
  filterModel: {
    type: String,
    default: ''
  }
})

defineEmits([
  'update:searchQuery',
  'update:filterStatus',
  'update:filterPlan',
  'update:filterModel',
  'reset-filters',
  'open-new-customer',
  'open-edit-modal',
  'open-license-modal',
  'quick-extend-1m',
  'quick-extend-1y',
  'toggle-lock',
  'delete-customer',
  'open-history',
  'open-backups'
])

const viewMode = ref('table') // 'table' | 'grid'

const now = new Date()

// Lọc danh sách khách hàng
const filteredCustomers = computed(() => {
  return props.customers.filter(c => {
    // 1. Tìm kiếm từ khóa
    if (props.searchQuery) {
      const q = props.searchQuery.toLowerCase().trim()
      const matchName = c.shopName?.toLowerCase().includes(q)
      const matchCode = c.shopCode?.toLowerCase().includes(q)
      const matchOwner = c.ownerName?.toLowerCase().includes(q)
      const matchPhone = c.phone?.toLowerCase().includes(q)
      const matchHw = c.hardwareId?.toLowerCase().includes(q)
      const matchAddress = c.address?.toLowerCase().includes(q)
      if (!matchName && !matchCode && !matchOwner && !matchPhone && !matchHw && !matchAddress) return false
    }

    // 2. Lọc trạng thái
    if (props.filterStatus) {
      const exp = new Date(c.expiresAt)
      if (props.filterStatus === 'Active') {
        if (c.status !== 'Active' || exp <= now) return false
      } else if (props.filterStatus === 'ExpiringSoon') {
        const soon = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000)
        if (c.status !== 'Active' || exp <= now || exp > soon) return false
      } else if (props.filterStatus === 'Expired') {
        if (c.status !== 'Expired' && exp > now) return false
      } else if (props.filterStatus === 'Suspended') {
        if (c.status !== 'Suspended') return false
      }
    }

    // 3. Lọc gói cước
    if (props.filterPlan) {
      if (c.currentPlan !== props.filterPlan) return false
    }

    // 4. Lọc mô hình
    if (props.filterModel) {
      if (c.businessModel !== props.filterModel) return false
    }

    return true
  })
})

function formatBusinessModel(model) {
  switch (model) {
    case 'Barber': return '✂️ Barber Tóc Nam'
    case 'Salon': return '💇‍♀️ Salon Tóc Nữ'
    case 'Spa': return '💅 Nail & Spa'
    case 'Retail': return '🛍️ Bán Lẻ'
    case 'Cafe': return '☕ Cafe / Trà Sữa'
    default: return model || 'Khác'
  }
}

function isOnline(c) {
  if (!c || !c.lastPingAt) return false
  const diff = Date.now() - new Date(c.lastPingAt).getTime()
  return diff < 5 * 60 * 1000
}

function formatLastPing(c) {
  if (!c?.lastPingAt) return 'Chưa kết nối'
  const diff = Math.floor((Date.now() - new Date(c.lastPingAt).getTime()) / 60000)
  if (diff < 1) return 'Vừa xong'
  if (diff < 60) return `${diff}p trước`
  const hours = Math.floor(diff / 60)
  if (hours < 24) return `${hours}h trước`
  return formatDate(c.lastPingAt)
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

function getDaysRemaining(c) {
  if (!c.expiresAt) return 0
  const exp = new Date(c.expiresAt)
  return Math.ceil((exp - now) / (1000 * 60 * 60 * 24))
}

function getDaysRemainingText(c) {
  if (c.status === 'Suspended') return 'Đang bị khóa'
  const days = getDaysRemaining(c)
  if (days < 0) return `Quá hạn ${Math.abs(days)} ngày`
  if (days === 0) return 'Hết hạn hôm nay'
  return `Còn ${days} ngày`
}

function getDaysRemainingClass(c) {
  if (c.status === 'Suspended') return 'text-rose-400 font-bold'
  const days = getDaysRemaining(c)
  if (days <= 0) return 'text-rose-400 font-bold'
  if (days <= 7) return 'text-amber-400 font-bold'
  return 'text-slate-400'
}

function getStatusText(c) {
  if (c.status === 'Suspended') return 'Đã Tạm Khóa'
  if (getDaysRemaining(c) <= 0) return 'Hết Hạn'
  return 'Đang Hoạt Động'
}

function getStatusBadgeClass(c) {
  if (c.status === 'Suspended') return 'bg-rose-500/10 text-rose-400 border-rose-500/20'
  if (getDaysRemaining(c) <= 0) return 'bg-amber-500/10 text-amber-400 border-amber-500/20'
  return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20'
}

function getStatusDotClass(c) {
  if (c.status === 'Suspended') return 'bg-rose-400'
  if (getDaysRemaining(c) <= 0) return 'bg-amber-400'
  return 'bg-emerald-400'
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
