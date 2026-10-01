<template>
  <div class="min-h-screen bg-slate-950 text-slate-100 flex flex-col font-sans selection:bg-indigo-600 selection:text-white">
    <!-- Top Master Navigation Bar -->
    <header class="border-b border-slate-800 bg-slate-900/80 backdrop-blur-md sticky top-0 z-40">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        
        <!-- Logo & Navigation Tabs -->
        <div class="flex items-center gap-6 sm:gap-8">
          <!-- Logo & Brand -->
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-2xl bg-gradient-to-br from-indigo-500 via-indigo-600 to-blue-600 flex items-center justify-center text-white font-black text-lg shadow-lg shadow-indigo-500/25 tracking-tight">
              DA
            </div>
            <div>
              <div class="flex items-center gap-1.5">
                <h1 class="text-base font-black tracking-tight text-white">
                  Diro<span class="text-indigo-400">Admin</span>
                </h1>
                <span class="px-2 py-0.5 rounded text-[9px] font-black bg-indigo-500/20 text-indigo-300 border border-indigo-500/30">
                  PORTAL
                </span>
              </div>
              <p class="text-[10px] text-slate-400 font-medium">Trung Tâm Bản Quyền & Giám Sát POS</p>
            </div>
          </div>

          <!-- Navigation Tabs Switcher -->
          <nav class="hidden md:flex items-center p-1 bg-slate-950/80 rounded-2xl border border-slate-800/80 text-xs">
            <button
              @click="activeTab = 'dashboard'"
              class="px-4 py-1.5 rounded-xl font-bold flex items-center gap-2 transition cursor-pointer"
              :class="activeTab === 'dashboard' ? 'bg-indigo-600 text-white shadow-md shadow-indigo-600/30' : 'text-slate-400 hover:text-slate-200'"
            >
              <LayoutDashboard class="w-4 h-4" />
              <span>Bảng Điều Khiển</span>
            </button>

            <button
              @click="activeTab = 'customers'"
              class="px-4 py-1.5 rounded-xl font-bold flex items-center gap-2 transition cursor-pointer relative"
              :class="activeTab === 'customers' ? 'bg-indigo-600 text-white shadow-md shadow-indigo-600/30' : 'text-slate-400 hover:text-slate-200'"
            >
              <Users class="w-4 h-4" />
              <span>Quản Lý Khách Hàng</span>
              <span 
                v-if="customers.length > 0"
                class="px-1.5 py-0.2 rounded-full text-[9px] font-bold"
                :class="activeTab === 'customers' ? 'bg-indigo-700 text-white' : 'bg-slate-800 text-slate-300'"
              >
                {{ customers.length }}
              </span>
            </button>

            <button
              @click="openHistory(null)"
              class="px-3.5 py-1.5 rounded-xl font-bold flex items-center gap-1.5 transition cursor-pointer text-slate-400 hover:text-slate-200"
              title="Xem toàn bộ lịch sử các lần gia hạn và tiền thu"
            >
              <History class="w-4 h-4" />
              <span>Lịch Sử Gia Hạn</span>
            </button>

            <button
              @click="activeTab = 'versions'"
              class="px-3.5 py-1.5 rounded-xl font-bold flex items-center gap-1.5 transition cursor-pointer"
              :class="activeTab === 'versions' ? 'bg-indigo-600 text-white shadow-md shadow-indigo-600/30' : 'text-slate-400 hover:text-slate-200'"
              title="Quản lý các bản cập nhật phần mềm DiroPos"
            >
              <Rocket class="w-4 h-4" />
              <span>Quản Lý Phiên Bản</span>
            </button>
          </nav>
        </div>

        <!-- Right Quick Actions -->
        <div class="flex items-center gap-3">
          <button
            @click="openNewCustomerModal"
            class="px-3.5 sm:px-4 py-2 rounded-xl bg-gradient-to-r from-indigo-600 to-blue-600 hover:from-indigo-500 hover:to-blue-500 text-white font-bold text-xs shadow-md shadow-indigo-600/30 transition flex items-center gap-1.5 cursor-pointer active:scale-95"
          >
            <Plus class="w-4 h-4" />
            <span class="hidden sm:inline">Thêm Quán Khách Hàng</span>
            <span class="sm:hidden">Thêm Quán</span>
          </button>

          <div class="h-6 w-px bg-slate-800"></div>

          <!-- Admin Profile Badge -->
          <div class="flex items-center gap-2 bg-slate-800/60 border border-slate-700/60 py-1.5 px-3 rounded-xl">
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

      <!-- Mobile Nav Tabs Bar -->
      <div class="md:hidden flex items-center justify-around border-t border-slate-800/80 bg-slate-950 px-4 py-2 text-xs">
        <button
          @click="activeTab = 'dashboard'"
          class="flex items-center gap-1.5 py-1 px-2.5 rounded-lg font-bold"
          :class="activeTab === 'dashboard' ? 'bg-indigo-600 text-white' : 'text-slate-400'"
        >
          <LayoutDashboard class="w-3.5 h-3.5" />
          <span>Dashboard</span>
        </button>
        <button
          @click="activeTab = 'customers'"
          class="flex items-center gap-1.5 py-1 px-2.5 rounded-lg font-bold"
          :class="activeTab === 'customers' ? 'bg-indigo-600 text-white' : 'text-slate-400'"
        >
          <Users class="w-3.5 h-3.5" />
          <span>Khách Hàng</span>
        </button>
        <button
          @click="openHistory(null)"
          class="flex items-center gap-1.5 py-1 px-2.5 rounded-lg font-bold text-slate-400 hover:text-white"
        >
          <History class="w-3.5 h-3.5" />
          <span>Lịch Sử</span>
        </button>
        <button
          @click="activeTab = 'versions'"
          class="flex items-center gap-1.5 py-1 px-2.5 rounded-lg font-bold"
          :class="activeTab === 'versions' ? 'bg-indigo-600 text-white' : 'text-slate-400'"
        >
          <Rocket class="w-3.5 h-3.5" />
          <span>Phiên Bản</span>
        </button>
      </div>
    </header>

    <!-- Main Content Body -->
    <main class="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8 space-y-6">
      <!-- TAB 1: DASHBOARD CHUYÊN SÂU -->
      <DashboardView
        v-if="activeTab === 'dashboard'"
        :customers="customers"
        :records="licenseRecords"
        :revenue="totalRevenue"
        :loading="loading"
        @refresh="loadData"
        @open-new-customer="openNewCustomerModal"
        @switch-tab="activeTab = $event"
        @quick-extend-1m="quickExtend1Month"
        @quick-extend-1y="quickExtend1Year"
        @open-license-modal="openLicenseModal"
        @open-history="openHistory"
      />

      <!-- TAB 2: QUẢN LÝ KHÁCH HÀNG ĐẦY ĐỦ -->
      <CustomersView
        v-else-if="activeTab === 'customers'"
        :customers="customers"
        v-model:searchQuery="filters.search"
        v-model:filterStatus="filters.status"
        v-model:filterPlan="filters.plan"
        v-model:filterModel="filters.model"
        @reset-filters="resetFilters"
        @open-new-customer="openNewCustomerModal"
        @open-edit-modal="openEditModal"
        @open-license-modal="openLicenseModal"
        @quick-extend-1m="quickExtend1Month"
        @quick-extend-1y="quickExtend1Year"
        @toggle-lock="toggleCustomerLock"
        @delete-customer="deleteCustomer"
        @open-history="openHistory"
        @open-backups="openCustomerBackups"
      />

      <!-- TAB 3: QUẢN LÝ PHIÊN BẢN CẬP NHẬT DIROPOS -->
      <VersionsView
        v-else-if="activeTab === 'versions'"
      />
    </main>

    <!-- Footer -->
    <footer class="border-t border-slate-800/80 bg-slate-900/40 py-4 text-center text-xs text-slate-500">
      <p>DiroAdmin Master Portal © 2026 • Hệ thống Phân Phối Bản Quyền & Giám Sát DiroPos Toàn Diện</p>
    </footer>

    <!-- MODAL 1: Gia Hạn Tự Động Đến Máy POS (Zero-Touch Auto-Renewal) -->
    <div
      v-if="showLicenseModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/75 backdrop-blur-xs animate-fade-in"
    >
      <div class="bg-slate-900 border border-slate-800 rounded-3xl max-w-lg w-full p-6 space-y-4 shadow-2xl text-slate-200 animate-scale-up">
        <div class="flex items-center justify-between border-b border-slate-800 pb-3">
          <div class="flex items-center gap-2.5">
            <div class="w-8 h-8 rounded-xl bg-gradient-to-br from-indigo-500 to-emerald-500 flex items-center justify-center text-white">
              <Zap class="w-4 h-4" />
            </div>
            <div>
              <h3 class="font-extrabold text-white text-sm">Gia Hạn Tự Động Cho {{ selectedCustomer?.shopName }}</h3>
              <p class="text-[10px] text-emerald-400 font-medium">Lệnh sẽ truyền trực tiếp đến máy POS — Khách không cần nhập key</p>
            </div>
          </div>
          <button @click="showLicenseModal = false" class="text-slate-500 hover:text-white p-1 cursor-pointer">✕</button>
        </div>

        <div class="space-y-4 text-xs">
          <!-- Thông tin máy khách -->
          <div class="bg-slate-950 p-3.5 rounded-2xl border border-slate-800 space-y-1.5">
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
            class="w-full py-3 rounded-2xl bg-gradient-to-r from-emerald-600 via-teal-600 to-indigo-600 hover:from-emerald-500 hover:to-indigo-500 text-white font-black text-xs shadow-lg shadow-emerald-600/30 transition flex items-center justify-center gap-2 cursor-pointer disabled:opacity-50"
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

    <!-- MODAL 2: Thêm Quán Khách Hàng Mới -->
    <div
      v-if="showNewCustomerModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/75 backdrop-blur-xs animate-fade-in"
    >
      <div class="bg-slate-900 border border-slate-800 rounded-3xl max-w-lg w-full p-6 space-y-4 shadow-2xl text-slate-200 animate-scale-up">
        <div class="flex items-center justify-between border-b border-slate-800 pb-3">
          <div class="flex items-center gap-2">
            <Store class="w-5 h-5 text-indigo-400" />
            <h3 class="font-extrabold text-white text-sm">Thêm Quán Khách Hàng Mới</h3>
          </div>
          <button @click="showNewCustomerModal = false" class="text-slate-500 hover:text-white p-1 cursor-pointer">✕</button>
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
              <label class="block text-slate-400 mb-1 font-medium">Mã Quán (Tự động nếu để trống)</label>
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
                <option value="Spa">Nail & Spa Thẩm Mỹ</option>
                <option value="Retail">Bán Lẻ / Cửa Hàng</option>
                <option value="Cafe">Quán Cafe / Trà Sữa</option>
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
              class="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 font-bold cursor-pointer"
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

    <!-- MODAL 3: Chỉnh Sửa Thông Tin Quán (Update / Edit Customer) -->
    <div
      v-if="showEditCustomerModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/75 backdrop-blur-xs animate-fade-in"
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
          <button @click="showEditCustomerModal = false" class="text-slate-500 hover:text-white p-1 cursor-pointer">✕</button>
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

    <!-- MODAL 4: Lịch Sử Gia Hạn & Doanh Thu -->
    <LicenseHistoryModal
      :is-open="showHistoryModal"
      :customer="historyCustomer"
      :records="historyRecords"
      @close="showHistoryModal = false"
      @delete-record="handleDeleteLicenseRecord"
      @clear-all-records="handleClearAllLicenseRecords"
    />

    <!-- MODAL 5: Bản Sao Lưu Cloud (Supabase) -->
    <CustomerBackupsModal
      :is-open="showBackupsModal"
      :customer="selectedBackupCustomer"
      @close="showBackupsModal = false"
    />

  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import api from './api'
import DashboardView from './components/DashboardView.vue'
import CustomersView from './components/CustomersView.vue'
import VersionsView from './components/VersionsView.vue'
import LicenseHistoryModal from './components/LicenseHistoryModal.vue'
import CustomerBackupsModal from './components/CustomerBackupsModal.vue'
import {
  LayoutDashboard,
  Users,
  Plus,
  Store,
  Zap,
  Edit3,
  Copy,
  History,
  Rocket
} from 'lucide-vue-next'

const activeTab = ref('dashboard') // 'dashboard' | 'customers'

const customers = ref([])
const licenseRecords = ref([])
const totalRevenue = ref(0)
const historyCustomer = ref(null)
const historyRecords = ref([])
const showHistoryModal = ref(false)

const selectedBackupCustomer = ref(null)
const showBackupsModal = ref(false)

const loading = ref(false)
const saving = ref(false)
const extending = ref(false)
const copied = ref(false)
const showManualKey = ref(false)

const filters = ref({
  search: '',
  status: '',
  plan: '',
  model: ''
})

const showLicenseModal = ref(false)
const showNewCustomerModal = ref(false)
const showEditCustomerModal = ref(false)
const selectedCustomer = ref(null)

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
  { label: '1 Tháng', months: 1, plan: 'Monthly', price: 99000 },
  { label: '3 Tháng', months: 3, plan: 'Monthly', price: 280000 },
  { label: '6 Tháng', months: 6, plan: 'Monthly', price: 500000 },
  { label: '1 Năm', months: 12, plan: 'Yearly', price: 990000 },
  { label: '2 Năm', months: 24, plan: 'Yearly', price: 1800000 },
  { label: 'Trọn Đời', months: 120, plan: 'Lifetime', price: 2000000 }
]

const licenseForm = ref({
  months: 1,
  planType: 'Monthly',
  price: 99000
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

function resetFilters() {
  filters.value.search = ''
  filters.value.status = ''
  filters.value.plan = ''
  filters.value.model = ''
}

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

function isOnline(c) {
  if (!c || !c.lastPingAt) return false
  const diff = Date.now() - new Date(c.lastPingAt).getTime()
  return diff < 5 * 60 * 1000
}

async function loadData() {
  loading.value = true
  try {
    const [custRes, statsRes, recs] = await Promise.all([
      api.getCustomers(),
      api.getStats(),
      api.getLicenseRecords()
    ])
    if (custRes?.data) customers.value = custRes.data
    if (recs) licenseRecords.value = recs
    if (statsRes?.data?.totalRevenue !== undefined) {
      totalRevenue.value = statsRes.data.totalRevenue
    } else if (recs) {
      totalRevenue.value = recs.reduce((sum, r) => sum + (Number(r.price) || 0), 0)
    }
  } catch (err) {
    console.error('Lỗi tải dữ liệu:', err)
  } finally {
    loading.value = false
  }
}

async function handleDeleteLicenseRecord(record) {
  try {
    await api.deleteLicenseRecord(record)
    historyRecords.value = historyRecords.value.filter(r => {
      if (record.id && r.id && r.id === record.id) return false
      return r !== record
    })
    await loadData()
  } catch (err) {
    alert('Lỗi xóa bản ghi: ' + (err.message || err))
  }
}

async function handleClearAllLicenseRecords() {
  try {
    await api.clearAllLicenseRecords()
    historyRecords.value = []
    totalRevenue.value = 0
    await loadData()
    alert('✅ Đã xóa toàn bộ lịch sử gia hạn thành công! Doanh thu đã được đặt lại về 0 ₫.')
  } catch (err) {
    alert('Lỗi xóa lịch sử: ' + (err.message || err))
  }
}

async function openHistory(c = null) {
  const target = (c && c.shopCode) ? c : null
  historyCustomer.value = target
  historyRecords.value = await api.getLicenseRecords(target ? target.id : null, target ? target.shopCode : null)
  showHistoryModal.value = true
}

function openCustomerBackups(c) {
  selectedBackupCustomer.value = c
  showBackupsModal.value = true
}

async function quickExtend1Month(c) {
  if (!confirm(`Xác nhận GIA HẠN NHANH +1 THÁNG (99.000 đ) trực tiếp đến máy POS quán "${c.shopName}"?`)) return
  try {
    const res = await api.quickExtend(c.id, { months: 1, price: 99000 })
    alert(res.data.message || 'Gia hạn thành công!')
    await loadData()
  } catch (err) {
    alert('Lỗi gia hạn: ' + (err.response?.data?.message || err.message))
  }
}

async function quickExtend1Year(c) {
  if (!confirm(`Xác nhận GIA HẠN NHANH +1 NĂM (990.000 đ) trực tiếp đến máy POS quán "${c.shopName}"?`)) return
  try {
    const res = await api.quickExtend(c.id, { months: 12, price: 990000 })
    alert(res.data.message || 'Gia hạn thành công!')
    await loadData()
  } catch (err) {
    alert('Lỗi gia hạn: ' + (err.response?.data?.message || err.message))
  }
}

function openLicenseModal(c) {
  selectedCustomer.value = c
  showManualKey.value = false
  licenseForm.value = {
    months: 1,
    planType: 'Monthly',
    price: 99000
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
