<template>
  <div class="min-h-screen bg-slate-950 text-slate-100 flex flex-col">
    <!-- Top Navigation Bar -->
    <header class="border-b border-slate-800 bg-slate-900/60 backdrop-blur-md sticky top-0 z-30">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        <!-- Logo & Brand -->
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-indigo-500 via-indigo-600 to-blue-600 flex items-center justify-center text-white font-black text-lg shadow-lg shadow-indigo-500/25 tracking-tight">
            DA
          </div>
          <div>
            <div class="flex items-center gap-2">
              <h1 class="text-base font-extrabold tracking-tight text-white">
                Diro<span class="text-indigo-400">Admin</span>
              </h1>
              <span class="px-2 py-0.5 rounded text-[10px] font-black bg-indigo-500/20 text-indigo-300 border border-indigo-500/30">
                MASTER PORTAL
              </span>
            </div>
            <p class="text-[11px] text-slate-400 font-medium">Trung Tâm Phân Phối Bản Quyền & Giám Sát Khách Hàng</p>
          </div>
        </div>

        <!-- Right Quick Actions -->
        <div class="flex items-center gap-3">
          <button
            @click="openNewCustomerModal"
            class="px-4 py-2 rounded-xl bg-gradient-to-r from-indigo-600 to-blue-600 hover:from-indigo-500 hover:to-blue-500 text-white font-bold text-xs shadow-md shadow-indigo-600/30 transition flex items-center gap-2 cursor-pointer active:scale-95"
          >
            <Plus class="w-4 h-4" />
            <span>Thêm Quán Khách Hàng</span>
          </button>

          <div class="h-6 w-px bg-slate-800"></div>

          <div class="flex items-center gap-2.5 bg-slate-800/60 border border-slate-700/60 py-1.5 px-3 rounded-xl">
            <div class="w-7 h-7 rounded-lg bg-indigo-500/20 text-indigo-400 flex items-center justify-center font-bold text-xs">
              AD
            </div>
            <div class="text-left hidden sm:block">
              <p class="text-xs font-bold text-slate-200">Admin</p>
              <p class="text-[10px] text-emerald-400 flex items-center gap-1 font-medium">
                <span class="w-1.5 h-1.5 rounded-full bg-emerald-400 animate-pulse"></span>
                Trực tuyến
              </p>
            </div>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content Area -->
    <main class="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
      
      <!-- 1. Stats Overview Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <!-- Card 1: Tổng Khách Hàng -->
        <div class="p-5 rounded-2xl bg-slate-900 border border-slate-800/80 shadow-xs relative overflow-hidden group">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-400">Tổng Quán Khách Hàng</span>
            <div class="w-8 h-8 rounded-lg bg-indigo-500/10 text-indigo-400 flex items-center justify-center">
              <Store class="w-4 h-4" />
            </div>
          </div>
          <div class="mt-3">
            <span class="text-2xl font-black tracking-tight text-white">{{ stats.totalCustomers }}</span>
            <span class="text-xs text-slate-500 ml-1.5 font-medium">tiệm</span>
          </div>
          <div class="absolute -right-4 -bottom-4 w-20 h-20 bg-indigo-500/5 rounded-full blur-xl pointer-events-none"></div>
        </div>

        <!-- Card 2: Đang Hoạt Động -->
        <div class="p-5 rounded-2xl bg-slate-900 border border-slate-800/80 shadow-xs relative overflow-hidden group">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-400">Đang Bán Hàng Ổn Định</span>
            <div class="w-8 h-8 rounded-lg bg-emerald-500/10 text-emerald-400 flex items-center justify-center">
              <CheckCircle2 class="w-4 h-4" />
            </div>
          </div>
          <div class="mt-3">
            <span class="text-2xl font-black tracking-tight text-emerald-400">{{ stats.activeCount }}</span>
            <span class="text-xs text-slate-500 ml-1.5 font-medium">tiệm active</span>
          </div>
        </div>

        <!-- Card 3: Sắp Hết Hạn -->
        <div class="p-5 rounded-2xl bg-slate-900 border border-slate-800/80 shadow-xs relative overflow-hidden group">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-400">Sắp Hết Hạn (Trong 7 ngày)</span>
            <div class="w-8 h-8 rounded-lg bg-amber-500/10 text-amber-400 flex items-center justify-center">
              <AlertTriangle class="w-4 h-4" />
            </div>
          </div>
          <div class="mt-3">
            <span class="text-2xl font-black tracking-tight text-amber-400">{{ stats.expiringSoonCount }}</span>
            <span class="text-xs text-slate-500 ml-1.5 font-medium">tiệm cần thu phí</span>
          </div>
        </div>

        <!-- Card 4: Tổng Doanh Thu Phần Mềm -->
        <div class="p-5 rounded-2xl bg-slate-900 border border-slate-800/80 shadow-xs relative overflow-hidden group">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-400">Tổng Thu Bản Quyền</span>
            <div class="w-8 h-8 rounded-lg bg-blue-500/10 text-blue-400 flex items-center justify-center">
              <DollarSign class="w-4 h-4" />
            </div>
          </div>
          <div class="mt-3">
            <span class="text-xl font-black tracking-tight text-blue-400">{{ formatCurrency(stats.totalRevenue) }}</span>
          </div>
        </div>
      </div>

      <!-- 2. Controls & Search & Filter Bar -->
      <div class="flex flex-col sm:flex-row items-center justify-between gap-3 bg-slate-900 p-4 rounded-2xl border border-slate-800">
        <!-- Search Input -->
        <div class="relative w-full sm:w-80">
          <input
            v-model="filters.search"
            @input="loadCustomers"
            type="text"
            placeholder="Tìm theo tên tiệm, chủ quán, SĐT, mã quán..."
            class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3.5 py-2 pl-9 text-xs text-slate-200 placeholder-slate-500 focus:outline-none focus:border-indigo-500"
          />
          <Search class="w-4 h-4 text-slate-500 absolute left-3 top-2.5" />
        </div>

        <!-- Filter Selects -->
        <div class="flex items-center gap-2.5 w-full sm:w-auto text-xs">
          <select
            v-model="filters.status"
            @change="loadCustomers"
            class="bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-300 focus:outline-none focus:border-indigo-500"
          >
            <option value="">Tất cả trạng thái</option>
            <option value="Active">Đang hoạt động</option>
            <option value="Suspended">Đã tạm khóa</option>
            <option value="Expired">Đã hết hạn</option>
          </select>

          <select
            v-model="filters.plan"
            @change="loadCustomers"
            class="bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-300 focus:outline-none focus:border-indigo-500"
          >
            <option value="">Tất cả gói cước</option>
            <option value="Trial">Dùng thử (Trial)</option>
            <option value="Monthly">Gói Tháng</option>
            <option value="Yearly">Gói Năm</option>
            <option value="Lifetime">Trọn đời (Lifetime)</option>
          </select>

          <button
            @click="loadData"
            class="p-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 transition"
            title="Làm mới dữ liệu"
          >
            <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': loading }" />
          </button>
        </div>
      </div>

      <!-- 3. Customers Table -->
      <div class="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-xs">
        <div class="overflow-x-auto">
          <table class="w-full text-left text-xs text-slate-300">
            <thead class="bg-slate-950/60 border-b border-slate-800 text-[11px] font-bold text-slate-400 uppercase tracking-wider">
              <tr>
                <th class="py-3.5 px-4">Mã Quán</th>
                <th class="py-3.5 px-4">Tiệm & Thiết Bị POS</th>
                <th class="py-3.5 px-4">Chủ Tiệm & SĐT</th>
                <th class="py-3.5 px-4">Gói Cước</th>
                <th class="py-3.5 px-4">Hạn Sử Dụng</th>
                <th class="py-3.5 px-4">Trạng Thái</th>
                <th class="py-3.5 px-4 text-right">Gia Hạn Nhanh & Thao Tác</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-800/60">
              <tr v-if="customers.length === 0" class="text-center py-10">
                <td colspan="7" class="py-12 text-slate-500">
                  <Store class="w-8 h-8 mx-auto mb-2 text-slate-600 stroke-1" />
                  <p class="font-medium">Chưa có quán khách hàng nào phù hợp bộ lọc</p>
                </td>
              </tr>

              <tr
                v-for="c in customers"
                :key="c.id"
                class="hover:bg-slate-800/40 transition group"
              >
                <!-- Mã Quán -->
                <td class="py-4 px-4 whitespace-nowrap">
                  <span class="font-mono font-bold text-indigo-400 bg-indigo-500/10 border border-indigo-500/20 px-2 py-1 rounded-lg">
                    {{ c.shopCode }}
                  </span>
                </td>

                <!-- Tên Tiệm & Thiết Bị POS -->
                <td class="py-4 px-4">
                  <div class="flex items-center gap-2">
                    <p class="font-bold text-white text-xs">{{ c.shopName }}</p>
                    <!-- Chấm Online / Offline -->
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
                      ID: {{ c.hardwareId || 'Chưa nhận diện máy' }}
                    </span>
                    <span v-if="c.lastPingAt" class="text-[9px]">
                      Ping: {{ formatLastPing(c) }}
                    </span>
                  </div>
                </td>

                <!-- Chủ Quán & SĐT -->
                <td class="py-4 px-4 whitespace-nowrap">
                  <p class="font-medium text-slate-200">{{ c.ownerName }}</p>
                  <a :href="'tel:' + c.phone" class="text-[11px] font-mono text-emerald-400 hover:underline">
                    {{ c.phone }}
                  </a>
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
                      @click="quickExtend1Month(c)"
                      title="Gia hạn nhanh +1 Tháng đến máy POS"
                      class="px-2 py-1 rounded-lg bg-emerald-600/20 hover:bg-emerald-600 text-emerald-400 hover:text-white transition font-bold text-[11px] flex items-center gap-1 cursor-pointer border border-emerald-500/30"
                    >
                      <Zap class="w-3 h-3" />
                      <span>+1T</span>
                    </button>

                    <!-- Nút Gia Hạn Nhanh 1 Năm -->
                    <button
                      @click="quickExtend1Year(c)"
                      title="Gia hạn nhanh +1 Năm đến máy POS"
                      class="px-2 py-1 rounded-lg bg-indigo-600/20 hover:bg-indigo-600 text-indigo-400 hover:text-white transition font-bold text-[11px] flex items-center gap-1 cursor-pointer border border-indigo-500/30"
                    >
                      <Zap class="w-3 h-3" />
                      <span>+1N</span>
                    </button>

                    <!-- Nút Mở Modal Gia Hạn Tùy Chọn -->
                    <button
                      @click="openLicenseModal(c)"
                      title="Tùy chọn gia hạn & xem chi tiết"
                      class="p-1.5 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-300 hover:text-white transition cursor-pointer"
                    >
                      <Key class="w-3.5 h-3.5" />
                    </button>

                    <!-- Nút Chỉnh Sửa Thông Tin Quán -->
                    <button
                      @click="openEditModal(c)"
                      title="Chỉnh sửa thông tin quán"
                      class="p-1.5 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-300 hover:text-white transition cursor-pointer"
                    >
                      <Edit3 class="w-3.5 h-3.5" />
                    </button>

                    <!-- Nút Khóa / Mở Khóa Tức Thì -->
                    <button
                      @click="toggleCustomerLock(c)"
                      :title="c.status === 'Suspended' ? 'Mở Khóa Quán Tức Thì' : 'Khóa Quán Tức Thì (Kill-switch)'"
                      class="p-1.5 rounded-lg transition cursor-pointer"
                      :class="c.status === 'Suspended' ? 'bg-emerald-600/20 text-emerald-400 hover:bg-emerald-600 hover:text-white' : 'bg-rose-600/20 text-rose-400 hover:bg-rose-600 hover:text-white'"
                    >
                      <Lock v-if="c.status !== 'Suspended'" class="w-3.5 h-3.5" />
                      <Unlock v-else class="w-3.5 h-3.5" />
                    </button>

                    <!-- Nút Xóa -->
                    <button
                      @click="deleteCustomer(c)"
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

    </main>

    <!-- Modal 1: Gia Hạn Tự Động Đến Máy POS (Zero-Touch Auto-Renewal) -->
    <div
      v-if="showLicenseModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-xs animate-fade-in"
    >
      <div class="bg-slate-900 border border-slate-800 rounded-3xl max-w-lg w-full p-6 space-y-4 shadow-2xl text-slate-200 animate-scale-up">
        <div class="flex items-center justify-between border-b border-slate-800 pb-3">
          <div class="flex items-center gap-2">
            <div class="w-8 h-8 rounded-xl bg-gradient-to-br from-indigo-500 to-emerald-500 flex items-center justify-center text-white">
              <Zap class="w-4 h-4" />
            </div>
            <div>
              <h3 class="font-extrabold text-white text-sm">Gia Hạn Tự Động Cho {{ selectedCustomer?.shopName }}</h3>
              <p class="text-[10px] text-emerald-400 font-medium">Lệnh sẽ truyền trực tiếp đến máy POS — Khách không cần nhập key</p>
            </div>
          </div>
          <button @click="showLicenseModal = false" class="text-slate-500 hover:text-white p-1">✕</button>
        </div>

        <div class="space-y-4 text-xs">
          <!-- Thông tin máy khách -->
          <div class="bg-slate-950 p-3.5 rounded-xl border border-slate-800 space-y-1.5">
            <div class="flex items-center justify-between">
              <span class="text-slate-400">Mã Quán: <b class="text-indigo-400 font-mono">{{ selectedCustomer?.shopCode }}</b></span>
              <span
                class="inline-flex items-center gap-1 px-1.5 py-0.5 rounded text-[9px] font-bold"
                :class="isOnline(selectedCustomer) ? 'bg-emerald-500/20 text-emerald-400' : 'bg-slate-800 text-slate-400'"
              >
                <span class="w-1.5 h-1.5 rounded-full" :class="isOnline(selectedCustomer) ? 'bg-emerald-400 animate-pulse' : 'bg-slate-500'"></span>
                {{ isOnline(selectedCustomer) ? 'Máy POS đang Online' : 'POS đang Offline (sẽ nhận khi mở mạng)' }}
              </span>
            </div>
            <p class="text-slate-300">Chủ tiệm: <b class="text-white">{{ selectedCustomer?.ownerName }}</b> ({{ selectedCustomer?.phone }})</p>
            <p class="text-slate-400 font-mono text-[11px]">Hardware ID: <span class="text-slate-200">{{ selectedCustomer?.hardwareId || 'Chưa nhận diện máy' }}</span></p>
            <p class="text-slate-400">Hạn dùng hiện tại: <b class="text-amber-300">{{ formatDate(selectedCustomer?.expiresAt) }}</b></p>
          </div>

          <!-- Chọn gói cấp -->
          <div>
            <label class="block text-slate-400 mb-1.5 font-medium">Chọn Gói Cước Gia Hạn *</label>
            <div class="grid grid-cols-3 gap-2">
              <button
                type="button"
                v-for="pkg in packages"
                :key="pkg.months"
                @click="licenseForm.months = pkg.months; licenseForm.planType = pkg.plan; licenseForm.price = pkg.price"
                class="p-2.5 rounded-xl border text-center transition cursor-pointer"
                :class="licenseForm.months === pkg.months ? 'bg-indigo-600 text-white font-bold border-indigo-500 shadow-md shadow-indigo-600/30' : 'bg-slate-950 border-slate-800 text-slate-300 hover:bg-slate-800'"
              >
                <p class="font-bold text-xs">{{ pkg.label }}</p>
                <p class="text-[10px] opacity-80 mt-0.5">{{ formatCurrency(pkg.price) }}</p>
              </button>
            </div>
          </div>

          <!-- Số tiền thu -->
          <div>
            <label class="block text-slate-400 mb-1 font-medium">Số tiền thu thực tế (VNĐ)</label>
            <input
              v-model="licenseForm.price"
              type="number"
              class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-white font-mono font-bold focus:outline-none focus:border-indigo-500"
            />
          </div>

          <!-- Nút kích hoạt gia hạn trực tiếp -->
          <button
            @click="submitAutoExtend"
            :disabled="extending"
            class="w-full py-3 rounded-xl bg-gradient-to-r from-emerald-600 via-teal-600 to-indigo-600 hover:from-emerald-500 hover:to-indigo-500 text-white font-black text-xs shadow-lg shadow-emerald-600/30 transition flex items-center justify-center gap-2 cursor-pointer disabled:opacity-50"
          >
            <Zap class="w-4 h-4" />
            <span>{{ extending ? 'Đang gửi lệnh gia hạn...' : `⚡ KÍCH HOẠT GIA HẠN +${licenseForm.months} THÁNG NGAY` }}</span>
          </button>

          <!-- Tùy chọn xem key thủ công nếu cần -->
          <div class="border-t border-slate-800/80 pt-3">
            <button
              type="button"
              @click="showManualKey = !showManualKey"
              class="text-[11px] text-slate-500 hover:text-slate-300 flex items-center gap-1 cursor-pointer"
            >
              <span>{{ showManualKey ? 'Ẩn mã key thủ công' : '👉 Nhấn để xem mã Key sao chép thủ công (dành cho quán offline)' }}</span>
            </button>

            <div v-if="showManualKey" class="mt-2.5 p-3 rounded-xl bg-slate-950 border border-slate-800 space-y-2">
              <div class="flex items-center justify-between text-indigo-300 font-bold text-[10px]">
                <span>KEY MÃ HÓA KÝ SỐ:</span>
                <button
                  v-if="selectedCustomer?.activeLicenseKey"
                  @click="copyKey(selectedCustomer.activeLicenseKey)"
                  class="bg-indigo-600 hover:bg-indigo-500 text-white px-2 py-0.5 rounded text-[10px] flex items-center gap-1 cursor-pointer"
                >
                  <Copy class="w-3 h-3" />
                  <span>{{ copied ? 'Đã chép!' : 'Sao chép' }}</span>
                </button>
              </div>
              <textarea
                readonly
                :value="selectedCustomer?.activeLicenseKey || 'Chưa tạo key'"
                rows="2"
                class="w-full bg-slate-900 border border-slate-800 rounded-lg p-2 font-mono text-[10px] text-indigo-200 select-all"
              ></textarea>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal 2: Thêm Quán Khách Hàng Mới -->
    <div
      v-if="showNewCustomerModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-xs animate-fade-in"
    >
      <div class="bg-slate-900 border border-slate-800 rounded-3xl max-w-lg w-full p-6 space-y-4 shadow-2xl text-slate-200 animate-scale-up">
        <div class="flex items-center justify-between border-b border-slate-800 pb-3">
          <div class="flex items-center gap-2">
            <Store class="w-5 h-5 text-indigo-400" />
            <h3 class="font-extrabold text-white text-sm">Thêm Quán Khách Hàng Mới</h3>
          </div>
          <button @click="showNewCustomerModal = false" class="text-slate-500 hover:text-white p-1">✕</button>
        </div>

        <form @submit.prevent="submitCreateCustomer" class="space-y-3.5 text-xs">
          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-slate-400 mb-1 font-medium">Tên Quán / Tiệm *</label>
              <input
                v-model="newCustomerForm.shopName"
                type="text"
                required
                placeholder="VD: Hải Barber Shop"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-indigo-500"
              />
            </div>
            <div>
              <label class="block text-slate-400 mb-1 font-medium">Mã Quán (Tự động hoặc tự đặt)</label>
              <input
                v-model="newCustomerForm.shopCode"
                type="text"
                placeholder="VD: DP-HAI-01"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-indigo-400 font-mono font-bold focus:outline-none focus:border-indigo-500"
              />
            </div>
          </div>

          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-slate-400 mb-1 font-medium">Chủ Tiệm</label>
              <input
                v-model="newCustomerForm.ownerName"
                type="text"
                placeholder="Họ tên chủ quán..."
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-indigo-500"
              />
            </div>
            <div>
              <label class="block text-slate-400 mb-1 font-medium">Số Điện Thoại (Zalo) *</label>
              <input
                v-model="newCustomerForm.phone"
                type="text"
                required
                placeholder="VD: 0988776655"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-emerald-400 font-mono font-bold focus:outline-none focus:border-indigo-500"
              />
            </div>
          </div>

          <div>
            <label class="block text-slate-400 mb-1 font-medium">Địa Chỉ Quán</label>
            <input
              v-model="newCustomerForm.address"
              type="text"
              placeholder="Số nhà, tên đường, quận huyện..."
              class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-white focus:outline-none focus:border-indigo-500"
            />
          </div>

          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-slate-400 mb-1 font-medium">Mô Hình</label>
              <select
                v-model="newCustomerForm.businessModel"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500"
              >
                <option value="Barber">Tiệm Tóc Nam (Barber)</option>
                <option value="Salon">Salon Tóc Nữ</option>
                <option value="Spa">Nail & Spa</option>
                <option value="Retail">Bán Lẻ / Cửa Hàng</option>
              </select>
            </div>
            <div>
              <label class="block text-slate-400 mb-1 font-medium">Gói Khởi Tạo</label>
              <select
                v-model="newCustomerForm.initialMonths"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500"
              >
                <option :value="1">Dùng thử 1 tháng (Trial)</option>
                <option :value="6">Gói 6 tháng</option>
                <option :value="12">Gói 1 năm</option>
              </select>
            </div>
          </div>

          <div class="flex items-center justify-end gap-2.5 pt-3 border-t border-slate-800">
            <button
              type="button"
              @click="showNewCustomerModal = false"
              class="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 font-bold"
            >
              Hủy
            </button>
            <button
              type="submit"
              :disabled="saving"
              class="px-5 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-bold shadow-md shadow-indigo-600/30 flex items-center gap-1.5 cursor-pointer disabled:opacity-50"
            >
              <span>{{ saving ? 'Đang tạo...' : 'Tạo Quán & Cấp Key Ngay' }}</span>
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal 3: Chỉnh Sửa Thông Tin Quán (Update / Edit Customer) -->
    <div
      v-if="showEditCustomerModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-xs animate-fade-in"
    >
      <div class="bg-slate-900 border border-slate-800 rounded-3xl max-w-lg w-full p-6 space-y-4 shadow-2xl text-slate-200 animate-scale-up max-h-[90vh] overflow-y-auto">
        <div class="flex items-center justify-between border-b border-slate-800 pb-3">
          <div class="flex items-center gap-2">
            <div class="w-8 h-8 rounded-xl bg-indigo-600/20 border border-indigo-500/30 flex items-center justify-center text-indigo-400">
              <Edit3 class="w-4 h-4" />
            </div>
            <div>
              <h3 class="font-extrabold text-white text-sm">Chỉnh Sửa Thông Tin Quán</h3>
              <p class="text-[10px] text-slate-400">Mã Quán: <b class="text-indigo-400 font-mono">{{ editCustomerForm.shopCode }}</b></p>
            </div>
          </div>
          <button @click="showEditCustomerModal = false" class="text-slate-500 hover:text-white p-1">✕</button>
        </div>

        <form @submit.prevent="submitEditCustomer" class="space-y-3.5 text-xs">
          <!-- Tên quán -->
          <div>
            <label class="block text-slate-400 mb-1 font-medium">Tên Tiệm / Quán <span class="text-rose-500">*</span></label>
            <input
              v-model="editCustomerForm.shopName"
              type="text"
              required
              class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500"
            />
          </div>

          <!-- Chủ tiệm & SĐT -->
          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-slate-400 mb-1 font-medium">Chủ Tiệm</label>
              <input
                v-model="editCustomerForm.ownerName"
                type="text"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500"
              />
            </div>
            <div>
              <label class="block text-slate-400 mb-1 font-medium">Số Điện Thoại</label>
              <input
                v-model="editCustomerForm.phone"
                type="text"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500"
              />
            </div>
          </div>

          <!-- Địa chỉ -->
          <div>
            <label class="block text-slate-400 mb-1 font-medium">Địa Chỉ Quán</label>
            <input
              v-model="editCustomerForm.address"
              type="text"
              class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500"
            />
          </div>

          <!-- Mô hình kinh doanh & Gói cước -->
          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-slate-400 mb-1 font-medium">Mô Hình</label>
              <select
                v-model="editCustomerForm.businessModel"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500"
              >
                <option value="Barber">Barber / Tiệm Cắt Tóc</option>
                <option value="Salon">Salon Tóc Nữ</option>
                <option value="Spa">Spa / Thẩm Mỹ / Nail</option>
                <option value="Retail">Cửa Hàng Bán Lẻ</option>
                <option value="Cafe">Quán Cafe / Trà Sữa</option>
                <option value="Other">Mô Hình Khác</option>
              </select>
            </div>

            <div>
              <label class="block text-slate-400 mb-1 font-medium">Gói Cước</label>
              <select
                v-model="editCustomerForm.currentPlan"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500"
              >
                <option value="Trial">Dùng Thử (Trial)</option>
                <option value="Monthly">Gói Tháng (Monthly)</option>
                <option value="Yearly">Gói Năm (Yearly)</option>
                <option value="Lifetime">Trọn Đời (Lifetime)</option>
              </select>
            </div>
          </div>

          <!-- Trạng thái & Ngày hết hạn -->
          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-slate-400 mb-1 font-medium">Trạng Thái Bản Quyền</label>
              <select
                v-model="editCustomerForm.status"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500 font-bold"
                :class="editCustomerForm.status === 'Active' ? 'text-emerald-400' : (editCustomerForm.status === 'Suspended' ? 'text-rose-400' : 'text-amber-400')"
              >
                <option value="Active">Đang Hoạt Động (Active)</option>
                <option value="Suspended">Đang Khóa (Suspended)</option>
                <option value="Expired">Hết Hạn (Expired)</option>
              </select>
            </div>

            <div>
              <label class="block text-slate-400 mb-1 font-medium">Hạn Sử Dụng</label>
              <input
                v-model="editCustomerForm.expiresAt"
                type="date"
                class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500 font-mono"
              />
            </div>
          </div>

          <!-- Hardware ID -->
          <div>
            <div class="flex items-center justify-between mb-1">
              <label class="text-slate-400 font-medium">Mã Thiết Bị (Hardware ID)</label>
              <button
                type="button"
                v-if="editCustomerForm.hardwareId"
                @click="editCustomerForm.hardwareId = ''"
                class="text-[10px] text-amber-400 hover:underline cursor-pointer"
              >
                Xóa ID để gán máy khác
              </button>
            </div>
            <input
              v-model="editCustomerForm.hardwareId"
              type="text"
              placeholder="VD: HW-F23B-A6BA-9D57 (để trống nếu chưa gắn máy)"
              class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500 font-mono text-[11px]"
            />
          </div>

          <!-- Ghi chú -->
          <div>
            <label class="block text-slate-400 mb-1 font-medium">Ghi Chú Quản Trị</label>
            <textarea
              v-model="editCustomerForm.notes"
              rows="2"
              placeholder="Ghi chú về khách hàng, thỏa thuận thanh toán..."
              class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3 py-2 text-slate-200 focus:outline-none focus:border-indigo-500"
            ></textarea>
          </div>

          <!-- Buttons -->
          <div class="flex items-center justify-end gap-2.5 pt-3 border-t border-slate-800">
            <button
              type="button"
              @click="showEditCustomerModal = false"
              class="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 font-bold cursor-pointer"
            >
              Hủy
            </button>
            <button
              type="submit"
              :disabled="saving"
              class="px-5 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-bold shadow-md shadow-indigo-600/30 flex items-center gap-1.5 cursor-pointer disabled:opacity-50"
            >
              <span>{{ saving ? 'Đang lưu...' : 'Lưu Thay Đổi' }}</span>
            </button>
          </div>
        </form>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import api from './api'
import {
  Store,
  CheckCircle2,
  AlertTriangle,
  DollarSign,
  Search,
  RefreshCw,
  Plus,
  Key,
  Lock,
  Unlock,
  Trash2,
  Copy,
  Zap,
  Edit3
} from 'lucide-vue-next'

const stats = ref({
  totalCustomers: 0,
  activeCount: 0,
  expiringSoonCount: 0,
  lockedOrExpiredCount: 0,
  totalRevenue: 0
})

const customers = ref([])
const loading = ref(false)
const saving = ref(false)
const generating = ref(false)
const extending = ref(false)
const copied = ref(false)
const showManualKey = ref(false)

const filters = ref({
  search: '',
  status: '',
  plan: ''
})

const showLicenseModal = ref(false)
const showNewCustomerModal = ref(false)
const showEditCustomerModal = ref(false)
const selectedCustomer = ref(null)
const generatedKey = ref('')

const editCustomerForm = ref({
  id: null,
  shopCode: '',
  shopName: '',
  ownerName: '',
  phone: '',
  address: '',
  businessModel: 'Barber',
  currentPlan: 'Monthly',
  status: 'Active',
  expiresAt: '',
  hardwareId: '',
  notes: ''
})

const packages = [
  { label: '1 Tháng', months: 1, plan: 'Monthly', price: 150000 },
  { label: '3 Tháng', months: 3, plan: 'Monthly', price: 420000 },
  { label: '6 Tháng', months: 6, plan: 'Monthly', price: 800000 },
  { label: '1 Năm', months: 12, plan: 'Yearly', price: 1500000 },
  { label: '2 Năm', months: 24, plan: 'Yearly', price: 2700000 },
  { label: 'Trọn Đời', months: 120, plan: 'Lifetime', price: 5000000 }
]

const licenseForm = ref({
  months: 12,
  planType: 'Yearly',
  price: 1500000
})

const newCustomerForm = ref({
  shopName: '',
  shopCode: '',
  ownerName: '',
  phone: '',
  address: '',
  businessModel: 'Barber',
  initialMonths: 1
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

function getDaysRemaining(c) {
  if (!c.expiresAt) return 0
  const now = new Date()
  const exp = new Date(c.expiresAt)
  const diff = Math.ceil((exp - now) / (1000 * 60 * 60 * 24))
  return diff
}

function getDaysRemainingText(c) {
  if (c.status === 'Suspended') return 'Đang bị khóa'
  const days = getDaysRemaining(c)
  if (days <= 0) return 'Đã hết hạn'
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

async function loadData() {
  loading.value = true
  try {
    await Promise.all([loadStats(), loadCustomers()])
  } finally {
    loading.value = false
  }
}

async function loadStats() {
  try {
    const res = await api.getStats()
    if (res?.data) stats.value = res.data
  } catch (err) {
    console.error('Lỗi tải thống kê:', err)
  }
}

async function loadCustomers() {
  try {
    const res = await api.getCustomers(filters.value)
    if (res?.data) customers.value = res.data
  } catch (err) {
    console.error('Lỗi tải khách hàng:', err)
  }
}

function isOnline(c) {
  if (!c || !c.lastPingAt) return false
  const diff = Date.now() - new Date(c.lastPingAt).getTime()
  return diff < 5 * 60 * 1000 // Trong vòng 5 phút coi là đang online
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

async function quickExtend1Month(c) {
  if (!confirm(`Xác nhận GIA HẠN NHANH +1 THÁNG trực tiếp đến máy POS quán "${c.shopName}"?`)) return
  try {
    const res = await api.quickExtend(c.id, { months: 1, price: 150000 })
    alert(res.data.message || 'Gia hạn thành công!')
    await loadData()
  } catch (err) {
    alert('Lỗi gia hạn: ' + (err.response?.data?.message || err.message))
  }
}

async function quickExtend1Year(c) {
  if (!confirm(`Xác nhận GIA HẠN NHANH +1 NĂM trực tiếp đến máy POS quán "${c.shopName}"?`)) return
  try {
    const res = await api.quickExtend(c.id, { months: 12, price: 1500000 })
    alert(res.data.message || 'Gia hạn thành công!')
    await loadData()
  } catch (err) {
    alert('Lỗi gia hạn: ' + (err.response?.data?.message || err.message))
  }
}

function openLicenseModal(c) {
  selectedCustomer.value = c
  generatedKey.value = c.activeLicenseKey || ''
  showManualKey.value = false
  licenseForm.value = {
    months: 12,
    planType: 'Yearly',
    price: 1500000
  }
  showLicenseModal.value = true
}

async function submitAutoExtend() {
  if (!selectedCustomer.value) return
  extending.value = true
  try {
    const res = await api.quickExtend(selectedCustomer.value.id, {
      months: licenseForm.value.months,
      price: licenseForm.value.price
    })
    alert(res.data.message || 'Đã kích hoạt gia hạn thành công đến máy POS!')
    showLicenseModal.value = false
    await loadData()
  } catch (err) {
    alert('Lỗi gia hạn: ' + (err.response?.data?.message || err.message))
  } finally {
    extending.value = false
  }
}

async function submitGenerateLicense() {
  if (!selectedCustomer.value) return
  generating.value = true
  try {
    const res = await api.generateLicense(selectedCustomer.value.id, licenseForm.value)
    if (res?.data) {
      generatedKey.value = res.data.licenseKey
      await loadData()
      alert('Đã sinh key bản quyền mới thành công!')
    }
  } catch (err) {
    alert('Lỗi tạo key: ' + (err.response?.data?.message || err.message))
  } finally {
    generating.value = false
  }
}

function copyKey(text) {
  navigator.clipboard.writeText(text)
  copied.value = true
  setTimeout(() => { copied.value = false }, 2500)
}

async function toggleCustomerLock(c) {
  const actionText = c.status === 'Suspended' ? 'MỞ KHÓA' : 'TẠM KHÓA'
  if (!confirm(`Bạn có chắc chắn muốn ${actionText} cho quán "${c.shopName}" không?`)) return

  try {
    await api.toggleLock(c.id)
    await loadData()
  } catch (err) {
    alert('Lỗi đổi trạng thái: ' + err.message)
  }
}

async function deleteCustomer(c) {
  if (!confirm(`Xác nhận xóa vĩnh viễn quán "${c.shopName}" (${c.shopCode}) khỏi hệ thống DiroAdmin?\n\nLưu ý: Máy POS của quán này sẽ lập tức bị chấm dứt bản quyền.`)) return
  try {
    await api.deleteCustomer(c.id)
    customers.value = customers.value.filter(item => item.id !== c.id)
    alert(`Đã xóa thành công quán "${c.shopName}"!`)
    await loadData()
  } catch (err) {
    alert('Lỗi xóa khách hàng: ' + (err.message || err))
  }
}

function openNewCustomerModal() {
  newCustomerForm.value = {
    shopName: '',
    shopCode: '',
    ownerName: '',
    phone: '',
    address: '',
    businessModel: 'Barber',
    initialMonths: 1
  }
  showNewCustomerModal.value = true
}

async function submitCreateCustomer() {
  saving.value = true
  try {
    const res = await api.createCustomer(newCustomerForm.value)
    if (res?.data) {
      showNewCustomerModal.value = false
      await loadData()
      alert(`Đã thêm thành công quán "${res.data.shopName}" với mã ${res.data.shopCode}!`)
    }
  } catch (err) {
    alert('Lỗi tạo quán: ' + (err.response?.data?.message || err.message))
  } finally {
    saving.value = false
  }
}

function openEditModal(c) {
  let expDate = ''
  if (c.expiresAt) {
    try {
      expDate = new Date(c.expiresAt).toISOString().split('T')[0]
    } catch {
      expDate = ''
    }
  }

  editCustomerForm.value = {
    id: c.id,
    shopCode: c.shopCode || '',
    shopName: c.shopName || '',
    ownerName: c.ownerName || '',
    phone: c.phone || '',
    address: c.address || '',
    businessModel: c.businessModel || 'Barber',
    currentPlan: c.currentPlan || 'Monthly',
    status: c.status || 'Active',
    expiresAt: expDate,
    hardwareId: c.hardwareId || '',
    notes: c.notes || ''
  }
  showEditCustomerModal.value = true
}

async function submitEditCustomer() {
  if (!editCustomerForm.value.shopName?.trim()) {
    alert('Vui lòng nhập tên quán.')
    return
  }

  saving.value = true
  try {
    const res = await api.updateCustomer(editCustomerForm.value.id, editCustomerForm.value)
    if (res?.data) {
      showEditCustomerModal.value = false
      await loadData()
      alert(`Đã cập nhật thông tin quán "${res.data.shopName}" thành công!`)
    }
  } catch (err) {
    alert('Lỗi cập nhật quán: ' + (err.response?.data?.message || err.message))
  } finally {
    saving.value = false
  }
}

let refreshTimer = null

onMounted(() => {
  loadData()
  try {
    api.subscribeCustomers(() => {
      loadData()
    })
  } catch (err) {
    console.warn('Realtime subscription error:', err)
  }

  // Tự động làm mới dữ liệu và trạng thái Online mỗi 20 giây
  refreshTimer = setInterval(() => {
    loadData()
  }, 20000)
})

onUnmounted(() => {
  if (refreshTimer) clearInterval(refreshTimer)
})
</script>

<style scoped>
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}
@keyframes scaleUp {
  from { transform: scale(0.95); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}
.animate-fade-in {
  animation: fadeIn 0.15s ease-out forwards;
}
.animate-scale-up {
  animation: scaleUp 0.2s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}
</style>
