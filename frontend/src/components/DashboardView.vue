<template>
  <div class="space-y-6 animate-fade-in">
    <!-- Header Welcome & Quick Info -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 bg-gradient-to-r from-slate-900 via-slate-900 to-indigo-950/40 p-5 rounded-3xl border border-slate-800/80 shadow-lg">
      <div class="space-y-1">
        <div class="flex items-center gap-2">
          <span class="px-2.5 py-0.5 rounded-full text-[10px] font-black uppercase bg-indigo-500/20 text-indigo-300 border border-indigo-500/30">
            Hệ Thống Quản Trị Đám Mây
          </span>
          <span class="text-xs text-slate-400">• Cập nhật thời gian thực</span>
        </div>
        <h2 class="text-xl sm:text-2xl font-black text-white tracking-tight">
          Bảng Điều Khiển Tổng Quan Hệ Thống 📊
        </h2>
        <p class="text-xs text-slate-400">
          Giám sát tình trạng bản quyền, doanh thu phần mềm và hoạt động của tất cả các tiệm POS trên toàn quốc.
        </p>
      </div>

      <div class="flex items-center gap-2.5">
        <button
          @click="$emit('open-new-customer')"
          class="px-4 py-2 rounded-xl bg-gradient-to-r from-indigo-600 to-blue-600 hover:from-indigo-500 hover:to-blue-500 text-white font-bold text-xs shadow-md shadow-indigo-600/30 transition flex items-center gap-1.5 cursor-pointer active:scale-95"
        >
          <Plus class="w-4 h-4" />
          <span>Thêm Quán Mới</span>
        </button>
        <button
          @click="$emit('refresh')"
          class="p-2.5 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 transition cursor-pointer"
          title="Làm mới dữ liệu"
        >
          <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': loading }" />
        </button>
      </div>
    </div>

    <!-- 1. 4 Thẻ KPI Chỉ Số Chính -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <!-- Card 1: Tổng Quán -->
      <div class="p-5 rounded-2xl bg-slate-900/90 border border-slate-800 shadow-sm relative overflow-hidden group hover:border-indigo-500/40 transition">
        <div class="flex items-center justify-between">
          <span class="text-xs font-semibold text-slate-400">Tổng Quán Khách Hàng</span>
          <div class="w-9 h-9 rounded-xl bg-indigo-500/10 text-indigo-400 flex items-center justify-center border border-indigo-500/20">
            <Store class="w-4 h-4" />
          </div>
        </div>
        <div class="mt-3 flex items-baseline gap-2">
          <span class="text-3xl font-black tracking-tight text-white">{{ customers.length }}</span>
          <span class="text-xs text-slate-400 font-medium">tiệm đăng ký</span>
        </div>
        <div class="mt-2 flex items-center gap-1.5 text-[11px] text-emerald-400 font-medium">
          <CheckCircle2 class="w-3.5 h-3.5" />
          <span>{{ activePercent }}% đang hoạt động ổn định</span>
        </div>
        <div class="absolute -right-4 -bottom-4 w-20 h-20 bg-indigo-500/5 rounded-full blur-xl pointer-events-none"></div>
      </div>

      <!-- Card 2: Máy POS Đang Online -->
      <div class="p-5 rounded-2xl bg-slate-900/90 border border-slate-800 shadow-sm relative overflow-hidden group hover:border-emerald-500/40 transition">
        <div class="flex items-center justify-between">
          <span class="text-xs font-semibold text-slate-400">Máy POS Đang Online</span>
          <div class="w-9 h-9 rounded-xl bg-emerald-500/10 text-emerald-400 flex items-center justify-center border border-emerald-500/20">
            <Activity class="w-4 h-4" />
          </div>
        </div>
        <div class="mt-3 flex items-baseline gap-2">
          <span class="text-3xl font-black tracking-tight text-emerald-400">{{ onlineCount }}</span>
          <span class="text-xs text-slate-400 font-medium">/ {{ customers.length }} tiệm kết nối</span>
        </div>
        <div class="mt-2 flex items-center gap-1.5 text-[11px] text-slate-400 font-medium">
          <span class="w-2 h-2 rounded-full bg-emerald-400 animate-pulse"></span>
          <span>Đồng bộ dữ liệu trong 5 phút qua</span>
        </div>
        <div class="absolute -right-4 -bottom-4 w-20 h-20 bg-emerald-500/5 rounded-full blur-xl pointer-events-none"></div>
      </div>

      <!-- Card 3: Cần Thu Phí / Sắp Hết Hạn -->
      <div class="p-5 rounded-2xl bg-slate-900/90 border border-slate-800 shadow-sm relative overflow-hidden group hover:border-amber-500/40 transition">
        <div class="flex items-center justify-between">
          <span class="text-xs font-semibold text-slate-400">Sắp Hết Hạn (Trong 7 ngày)</span>
          <div class="w-9 h-9 rounded-xl bg-amber-500/10 text-amber-400 flex items-center justify-center border border-amber-500/20">
            <AlertTriangle class="w-4 h-4" />
          </div>
        </div>
        <div class="mt-3 flex items-baseline gap-2">
          <span class="text-3xl font-black tracking-tight" :class="expiringSoonCount > 0 ? 'text-amber-400' : 'text-slate-300'">
            {{ expiringSoonCount }}
          </span>
          <span class="text-xs text-slate-400 font-medium">quán cần chăm sóc</span>
        </div>
        <div class="mt-2 flex items-center gap-1.5 text-[11px] text-amber-400/90 font-medium">
          <Clock class="w-3.5 h-3.5" />
          <span>{{ expiredCount }} quán đã quá hạn cần mở lại</span>
        </div>
        <div class="absolute -right-4 -bottom-4 w-20 h-20 bg-amber-500/5 rounded-full blur-xl pointer-events-none"></div>
      </div>

      <!-- Card 4: Tổng Doanh Thu Bản Quyền -->
      <div class="p-5 rounded-2xl bg-slate-900/90 border border-slate-800 shadow-sm relative overflow-hidden group hover:border-blue-500/40 transition">
        <div class="flex items-center justify-between">
          <span class="text-xs font-semibold text-slate-400">Tổng Thu Bản Quyền</span>
          <button
            @click="$emit('open-history')"
            class="text-[10px] font-bold text-blue-400 hover:text-blue-300 bg-blue-500/10 border border-blue-500/20 px-2 py-0.5 rounded cursor-pointer"
            title="Xem chi tiết lịch sử tất cả các lần gia hạn"
          >
            Lịch Sử 📜
          </button>
        </div>
        <div class="mt-3 flex items-baseline justify-between">
          <span class="text-2xl font-black tracking-tight text-blue-400">{{ formatCurrency(displayRevenue) }}</span>
          <span class="text-[10px] text-slate-400 font-mono">{{ records.length }} lượt gia hạn</span>
        </div>
        <div class="mt-2 flex items-center gap-1.5 text-[11px] text-blue-400/80 font-medium">
          <TrendingUp class="w-3.5 h-3.5" />
          <span>Ước tính MRR (99k/tháng): {{ formatCurrency(estimatedMRR) }}/tháng</span>
        </div>
        <div class="absolute -right-4 -bottom-4 w-20 h-20 bg-blue-500/5 rounded-full blur-xl pointer-events-none"></div>
      </div>
    </div>

    <!-- 1.5. KHỐI THEO DÕI TĂNG TRƯỞNG THEO THÁNG (MONTHLY COHORT & RETENTION) -->
    <div class="p-5 rounded-3xl bg-slate-900 border border-slate-800 space-y-4">
      <!-- Header với Bộ Chọn Tháng -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 border-b border-slate-800/80 pb-4">
        <div>
          <div class="flex items-center gap-2">
            <span class="w-2 h-2 rounded-full bg-indigo-400"></span>
            <h3 class="font-black text-white text-base tracking-tight flex items-center gap-2">
              <Calendar class="w-4 h-4 text-indigo-400" />
              <span>Chỉ Số Tăng Trưởng Hàng Tháng (Monthly Cohort)</span>
            </h3>
            <span 
              v-if="isCurrentMonthSelected"
              class="px-2 py-0.5 rounded-full text-[10px] font-bold bg-emerald-500/20 text-emerald-300 border border-emerald-500/30"
            >
              Tháng Hiện Tại
            </span>
          </div>
          <p class="text-xs text-slate-400 mt-0.5">
            Theo dõi tỷ lệ khách mới, lượt gia hạn tiếp tục và khách quá hạn để kịp thời chăm sóc giữ chân khách.
          </p>
        </div>

        <!-- Bộ Chuyển Tháng -->
        <div class="flex items-center gap-1.5 bg-slate-950 p-1.5 rounded-2xl border border-slate-800 self-start sm:self-auto">
          <button
            @click="prevMonth"
            class="p-1.5 rounded-xl hover:bg-slate-800 text-slate-400 hover:text-white transition cursor-pointer"
            title="Tháng trước"
          >
            <ChevronLeft class="w-4 h-4" />
          </button>
          
          <div class="px-3 py-1 font-bold text-xs text-white tracking-wide font-mono min-w-28 text-center">
            Tháng {{ String(selectedMonth).padStart(2, '0') }}/{{ selectedYear }}
          </div>

          <button
            @click="nextMonth"
            class="p-1.5 rounded-xl hover:bg-slate-800 text-slate-400 hover:text-white transition cursor-pointer"
            title="Tháng sau"
          >
            <ChevronRight class="w-4 h-4" />
          </button>

          <button
            v-if="!isCurrentMonthSelected"
            @click="resetToCurrentMonth"
            class="ml-1 px-2.5 py-1 rounded-xl bg-indigo-600/30 hover:bg-indigo-600/50 text-indigo-300 border border-indigo-500/30 font-bold text-[10px] transition cursor-pointer"
          >
            Hiện tại
          </button>
        </div>
      </div>

      <!-- 4 Thẻ Phân Tích Tháng -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3.5">
        <!-- 1. Khách Hàng Mới -->
        <div 
          @click="openCohortList('new')"
          class="p-4 rounded-2xl bg-gradient-to-br from-slate-950 to-emerald-950/20 border border-emerald-500/30 hover:border-emerald-500/60 transition cursor-pointer group shadow-sm"
        >
          <div class="flex items-center justify-between">
            <span class="text-xs font-bold text-emerald-400 flex items-center gap-1.5">
              <UserPlus class="w-3.5 h-3.5" />
              <span>Khách Hàng Mới</span>
            </span>
            <span class="text-[10px] font-bold text-emerald-400/80 bg-emerald-500/10 px-2 py-0.5 rounded group-hover:bg-emerald-500/20 transition">
              Xem chi tiết ➜
            </span>
          </div>
          <div class="mt-3 flex items-baseline gap-2">
            <span class="text-2xl font-black text-white">{{ newCustomersInMonth.length }}</span>
            <span class="text-xs text-slate-400">tiệm mới mở</span>
          </div>
          <p class="text-[11px] text-slate-400 mt-1">
            Đăng ký và kích hoạt trong tháng {{ selectedMonth }}/{{ selectedYear }}
          </p>
        </div>

        <!-- 2. Lượt Gia Hạn -->
        <div 
          @click="openCohortList('renewed')"
          class="p-4 rounded-2xl bg-gradient-to-br from-slate-950 to-blue-950/20 border border-blue-500/30 hover:border-blue-500/60 transition cursor-pointer group shadow-sm"
        >
          <div class="flex items-center justify-between">
            <span class="text-xs font-bold text-blue-400 flex items-center gap-1.5">
              <Repeat class="w-3.5 h-3.5" />
              <span>Lượt Gia Hạn</span>
            </span>
            <span class="text-[10px] font-bold text-blue-400/80 bg-blue-500/10 px-2 py-0.5 rounded group-hover:bg-blue-500/20 transition">
              Xem chi tiết ➜
            </span>
          </div>
          <div class="mt-3 flex items-baseline justify-between">
            <div class="flex items-baseline gap-1.5">
              <span class="text-2xl font-black text-white">{{ renewalsInMonth.length }}</span>
              <span class="text-xs text-slate-400">lượt đóng phí</span>
            </div>
            <span class="text-xs font-black text-emerald-400 font-mono">
              +{{ formatCurrency(renewalRevenueInMonth) }}
            </span>
          </div>
          <p class="text-[11px] text-slate-400 mt-1">
            Tổng tiền bản quyền thu được trong tháng
          </p>
        </div>

        <!-- 3. Khách Quá Hạn / Bỏ (Churned) -->
        <div 
          @click="openCohortList('churned')"
          class="p-4 rounded-2xl bg-gradient-to-br from-slate-950 to-rose-950/20 border border-rose-500/30 hover:border-rose-500/60 transition cursor-pointer group shadow-sm"
        >
          <div class="flex items-center justify-between">
            <span class="text-xs font-bold text-rose-400 flex items-center gap-1.5">
              <UserMinus class="w-3.5 h-3.5" />
              <span>Quá Hạn / Bỏ Quán</span>
            </span>
            <span class="text-[10px] font-bold text-rose-400/80 bg-rose-500/10 px-2 py-0.5 rounded group-hover:bg-rose-500/20 transition">
              Cứu khách ➜
            </span>
          </div>
          <div class="mt-3 flex items-baseline gap-2">
            <span class="text-2xl font-black text-rose-400">{{ churnedCustomersInMonth.length }}</span>
            <span class="text-xs text-slate-400">tiệm chưa gia hạn</span>
          </div>
          <p class="text-[11px] text-slate-400 mt-1">
            Hết hạn trong tháng này nhưng chưa đóng tiếp
          </p>
        </div>

        <!-- 4. Tăng Trưởng Ròng (Net Growth) -->
        <div class="p-4 rounded-2xl bg-slate-950 border border-slate-800 shadow-sm">
          <div class="flex items-center justify-between">
            <span class="text-xs font-bold text-slate-400">Tăng Trưởng Ròng</span>
            <span 
              class="text-[10px] font-extrabold px-2 py-0.5 rounded"
              :class="netGrowth >= 0 ? 'bg-emerald-500/20 text-emerald-300' : 'bg-rose-500/20 text-rose-300'"
            >
              {{ netGrowth >= 0 ? 'TĂNG TRƯỞNG +' : 'SUY GIẢM -' }}
            </span>
          </div>
          <div class="mt-3 flex items-baseline gap-2">
            <span 
              class="text-2xl font-black tracking-tight"
              :class="netGrowth >= 0 ? 'text-emerald-400' : 'text-rose-400'"
            >
              {{ netGrowth > 0 ? `+${netGrowth}` : netGrowth }}
            </span>
            <span class="text-xs text-slate-400">tiệm thực tăng</span>
          </div>
          <p class="text-[11px] text-slate-400 mt-1">
            Công thức: Khách mới ({{ newCustomersInMonth.length }}) - Khách bỏ ({{ churnedCustomersInMonth.length }})
          </p>
        </div>
      </div>
    </div>

    <!-- 2. Biểu Đồ & Phân Tích Cơ Cấu (Charts & Breakdown) -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-5">
      
      <!-- Cột 1 & 2: Cơ Cấu Gói Cước & Tỉ Lệ Sức Khỏe Bản Quyền -->
      <div class="lg:col-span-2 p-5 rounded-3xl bg-slate-900 border border-slate-800 space-y-5">
        <div class="flex items-center justify-between border-b border-slate-800/80 pb-3">
          <div>
            <h3 class="font-bold text-white text-sm flex items-center gap-2">
              <ShieldCheck class="w-4 h-4 text-indigo-400" />
              <span>Phân Bố Gói Cước & Tình Trạng Bản Quyền</span>
            </h3>
            <p class="text-[11px] text-slate-400">Tỉ lệ sử dụng giữa các gói Trial, Tháng, Năm và Trọn Đời</p>
          </div>
          <span class="text-xs text-slate-400 font-mono font-bold">{{ customers.length }} tổng quán</span>
        </div>

        <!-- Thanh Progress Bar Đa Sắc Phân Bổ Gói Cước -->
        <div class="space-y-2">
          <div class="flex justify-between text-xs text-slate-400 font-medium">
            <span>Tỉ lệ gói cước</span>
            <span class="text-slate-300">100% người dùng</span>
          </div>
          <div class="h-3 w-full bg-slate-950 rounded-full overflow-hidden flex p-0.5 border border-slate-800">
            <div 
              v-if="planDistribution.Trial" 
              :style="{ width: planDistribution.Trial.percent + '%' }"
              class="h-full bg-slate-500 rounded-l-full transition-all duration-500" 
              :title="`Trial: ${planDistribution.Trial.count} tiệm (${planDistribution.Trial.percent}%)`"
            ></div>
            <div 
              v-if="planDistribution.Monthly" 
              :style="{ width: planDistribution.Monthly.percent + '%' }"
              class="h-full bg-blue-500 transition-all duration-500" 
              :title="`Gói Tháng: ${planDistribution.Monthly.count} tiệm (${planDistribution.Monthly.percent}%)`"
            ></div>
            <div 
              v-if="planDistribution.Yearly" 
              :style="{ width: planDistribution.Yearly.percent + '%' }"
              class="h-full bg-indigo-500 transition-all duration-500" 
              :title="`Gói Năm: ${planDistribution.Yearly.count} tiệm (${planDistribution.Yearly.percent}%)`"
            ></div>
            <div 
              v-if="planDistribution.Lifetime" 
              :style="{ width: planDistribution.Lifetime.percent + '%' }"
              class="h-full bg-purple-500 rounded-r-full transition-all duration-500" 
              :title="`Trọn Đời: ${planDistribution.Lifetime.count} tiệm (${planDistribution.Lifetime.percent}%)`"
            ></div>
          </div>
        </div>

        <!-- Chi Tiết 4 Gói -->
        <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 text-xs">
          <!-- Trial -->
          <div class="p-3 rounded-2xl bg-slate-950 border border-slate-800/80">
            <div class="flex items-center gap-1.5 text-slate-400">
              <span class="w-2.5 h-2.5 rounded-full bg-slate-500"></span>
              <span class="font-bold">Dùng Thử (Trial)</span>
            </div>
            <div class="mt-2 flex items-baseline justify-between">
              <span class="text-lg font-black text-white">{{ planDistribution.Trial?.count || 0 }}</span>
              <span class="text-[11px] text-slate-400 font-mono">{{ planDistribution.Trial?.percent || 0 }}%</span>
            </div>
          </div>

          <!-- Monthly -->
          <div class="p-3 rounded-2xl bg-slate-950 border border-slate-800/80">
            <div class="flex items-center gap-1.5 text-blue-400">
              <span class="w-2.5 h-2.5 rounded-full bg-blue-500"></span>
              <span class="font-bold">Gói Tháng</span>
            </div>
            <div class="mt-2 flex items-baseline justify-between">
              <span class="text-lg font-black text-white">{{ planDistribution.Monthly?.count || 0 }}</span>
              <span class="text-[11px] text-blue-400 font-mono">{{ planDistribution.Monthly?.percent || 0 }}%</span>
            </div>
          </div>

          <!-- Yearly -->
          <div class="p-3 rounded-2xl bg-slate-950 border border-slate-800/80">
            <div class="flex items-center gap-1.5 text-indigo-400">
              <span class="w-2.5 h-2.5 rounded-full bg-indigo-500"></span>
              <span class="font-bold">Gói Năm</span>
            </div>
            <div class="mt-2 flex items-baseline justify-between">
              <span class="text-lg font-black text-white">{{ planDistribution.Yearly?.count || 0 }}</span>
              <span class="text-[11px] text-indigo-400 font-mono">{{ planDistribution.Yearly?.percent || 0 }}%</span>
            </div>
          </div>

          <!-- Lifetime -->
          <div class="p-3 rounded-2xl bg-slate-950 border border-slate-800/80">
            <div class="flex items-center gap-1.5 text-purple-400">
              <span class="w-2.5 h-2.5 rounded-full bg-purple-500"></span>
              <span class="font-bold">Trọn Đời</span>
            </div>
            <div class="mt-2 flex items-baseline justify-between">
              <span class="text-lg font-black text-white">{{ planDistribution.Lifetime?.count || 0 }}</span>
              <span class="text-[11px] text-purple-400 font-mono">{{ planDistribution.Lifetime?.percent || 0 }}%</span>
            </div>
          </div>
        </div>

        <!-- Trạng Thái Sức Khỏe Hệ Thống -->
        <div class="pt-2 border-t border-slate-800/80 grid grid-cols-1 sm:grid-cols-3 gap-3 text-xs">
          <div class="flex items-center gap-2.5 p-2 rounded-xl bg-slate-950/60">
            <div class="w-2.5 h-2.5 rounded-full bg-emerald-400"></div>
            <div>
              <p class="font-bold text-slate-200">{{ activeCount }} Tiệm Hoạt Động</p>
              <p class="text-[10px] text-slate-400">Đầy đủ bản quyền hợp lệ</p>
            </div>
          </div>
          <div class="flex items-center gap-2.5 p-2 rounded-xl bg-slate-950/60">
            <div class="w-2.5 h-2.5 rounded-full bg-amber-400"></div>
            <div>
              <p class="font-bold text-slate-200">{{ expiringSoonCount }} Tiệm Sắp Hết Hạn</p>
              <p class="text-[10px] text-slate-400">Hết hạn trong 7 ngày tới</p>
            </div>
          </div>
          <div class="flex items-center gap-2.5 p-2 rounded-xl bg-slate-950/60">
            <div class="w-2.5 h-2.5 rounded-full bg-rose-400"></div>
            <div>
              <p class="font-bold text-slate-200">{{ suspendedOrExpiredCount }} Tiệm Hết Hạn/Khóa</p>
              <p class="text-[10px] text-slate-400">Cần gia hạn hoặc mở khóa</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Cột 3: Cơ Cấu Mô Hình Kinh Doanh (Industry Breakdown) -->
      <div class="p-5 rounded-3xl bg-slate-900 border border-slate-800 space-y-4">
        <div class="border-b border-slate-800/80 pb-3">
          <h3 class="font-bold text-white text-sm flex items-center gap-2">
            <Store class="w-4 h-4 text-emerald-400" />
            <span>Mô Hình Tiệm Sử Dụng</span>
          </h3>
          <p class="text-[11px] text-slate-400">Phân bố ngành nghề khách hàng</p>
        </div>

        <div class="space-y-3">
          <div 
            v-for="model in businessModelStats" 
            :key="model.name"
            class="space-y-1 text-xs"
          >
            <div class="flex justify-between items-center text-slate-300">
              <span class="font-medium flex items-center gap-1.5">
                <span>{{ model.icon }}</span>
                <span>{{ model.name }}</span>
              </span>
              <span class="font-mono font-bold text-slate-200">{{ model.count }} tiệm ({{ model.percent }}%)</span>
            </div>
            <div class="h-2 w-full bg-slate-950 rounded-full overflow-hidden border border-slate-800/80">
              <div 
                class="h-full rounded-full transition-all duration-500"
                :class="model.color"
                :style="{ width: model.percent + '%' }"
              ></div>
            </div>
          </div>
        </div>

        <div class="pt-3 border-t border-slate-800/80 text-[11px] text-slate-400 bg-slate-950/60 p-3 rounded-2xl">
          💡 <b class="text-slate-200">Mẹo:</b> Mô hình <b>Barber & Salon Tóc</b> hiện đang chiếm tỉ trọng lớn nhất, nên ưu tiên các tính năng tích điểm và hoa hồng thợ.
        </div>
      </div>

    </div>

    <!-- 3. Danh Sách Quán Cần Gia Hạn Gấp (Urgent Renewal Action List) -->
    <div class="p-5 rounded-3xl bg-slate-900 border border-slate-800 space-y-4 shadow-sm">
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-2 border-b border-slate-800/80 pb-3">
        <div class="flex items-center gap-2.5">
          <div class="w-8 h-8 rounded-xl bg-amber-500/10 text-amber-400 flex items-center justify-center border border-amber-500/20">
            <AlertTriangle class="w-4 h-4" />
          </div>
          <div>
            <h3 class="font-bold text-white text-sm">Quán Cần Gia Hạn / Thu Phí Ngay ({{ urgentCustomers.length }})</h3>
            <p class="text-[11px] text-slate-400">Các tiệm sắp hết hạn trong 7 ngày tới hoặc đã quá hạn — Gia hạn trực tiếp tại đây</p>
          </div>
        </div>

        <button 
          @click="$emit('switch-tab', 'customers')"
          class="text-xs text-indigo-400 hover:text-indigo-300 font-semibold flex items-center gap-1 cursor-pointer"
        >
          <span>Xem tất cả khách hàng</span>
          <ArrowUpRight class="w-3.5 h-3.5" />
        </button>
      </div>

      <!-- Table hoặc Empty -->
      <div v-if="urgentCustomers.length === 0" class="py-8 text-center text-slate-500 text-xs">
        <CheckCircle2 class="w-8 h-8 text-emerald-400/60 mx-auto mb-2" />
        <p class="font-semibold text-slate-300">Tuyệt vời! Không có quán nào sắp hết hạn trong 7 ngày tới.</p>
        <p class="text-[11px] text-slate-500 mt-0.5">Tất cả tiệm đều đang trong thời hạn bản quyền an toàn.</p>
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full text-left text-xs text-slate-300">
          <thead class="bg-slate-950/60 border-b border-slate-800 text-[11px] font-bold text-slate-400 uppercase tracking-wider">
            <tr>
              <th class="py-3 px-3.5">Mã Quán</th>
              <th class="py-3 px-3.5">Tên Tiệm</th>
              <th class="py-3 px-3.5">Chủ Tiệm & SĐT</th>
              <th class="py-3 px-3.5">Gói Hiện Tại</th>
              <th class="py-3 px-3.5">Hạn Dùng & Cảnh Báo</th>
              <th class="py-3 px-3.5 text-right">Gia Hạn Nhanh Tức Thì</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-800/60">
            <tr 
              v-for="c in urgentCustomers" 
              :key="c.id"
              class="hover:bg-slate-800/40 transition"
            >
              <td class="py-3 px-3.5 whitespace-nowrap">
                <span class="font-mono font-bold text-indigo-400 bg-indigo-500/10 border border-indigo-500/20 px-2 py-0.5 rounded">
                  {{ c.shopCode }}
                </span>
              </td>
              <td class="py-3 px-3.5">
                <div class="flex items-center gap-1.5">
                  <p class="font-bold text-white">{{ c.shopName }}</p>
                  <span 
                    class="w-2 h-2 rounded-full" 
                    :class="isOnline(c) ? 'bg-emerald-400' : 'bg-slate-600'"
                    :title="isOnline(c) ? 'POS Online' : 'POS Offline'"
                  ></span>
                </div>
                <p class="text-[10px] text-slate-400 truncate max-w-xs">{{ c.address || 'Chưa có địa chỉ' }}</p>
              </td>
              <td class="py-3 px-3.5 whitespace-nowrap">
                <p class="text-slate-200 font-medium">{{ c.ownerName }}</p>
                <a :href="'tel:' + c.phone" class="text-[11px] font-mono text-emerald-400 hover:underline">
                  📞 {{ c.phone }}
                </a>
              </td>
              <td class="py-3 px-3.5 whitespace-nowrap">
                <span class="px-2 py-0.5 rounded text-[10px] font-extrabold uppercase border bg-slate-800 text-slate-300 border-slate-700">
                  {{ c.currentPlan }}
                </span>
              </td>
              <td class="py-3 px-3.5 whitespace-nowrap">
                <p class="font-bold text-slate-200">{{ formatDate(c.expiresAt) }}</p>
                <p class="text-[10px] font-bold" :class="getDaysRemainingClass(c)">
                  {{ getDaysRemainingText(c) }}
                </p>
              </td>
              <td class="py-3 px-3.5 text-right whitespace-nowrap space-x-1.5">
                <button
                  @click="$emit('quick-extend-1m', c)"
                  class="px-2.5 py-1 rounded-lg bg-emerald-600/20 hover:bg-emerald-600 text-emerald-400 hover:text-white transition font-bold text-[11px] cursor-pointer border border-emerald-500/30"
                  title="Gia hạn +1 Tháng trực tiếp đến POS"
                >
                  ⚡ +1 Tháng
                </button>
                <button
                  @click="$emit('quick-extend-1y', c)"
                  class="px-2.5 py-1 rounded-lg bg-indigo-600/20 hover:bg-indigo-600 text-indigo-400 hover:text-white transition font-bold text-[11px] cursor-pointer border border-indigo-500/30"
                  title="Gia hạn +1 Năm trực tiếp đến POS"
                >
                  ⚡ +1 Năm
                </button>
                <button
                  @click="$emit('open-license-modal', c)"
                  class="p-1 rounded-lg bg-slate-800 hover:bg-slate-700 text-slate-300 transition cursor-pointer"
                  title="Tùy chọn gia hạn khác"
                >
                  <Key class="w-3.5 h-3.5" />
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- 4. Danh Sách Máy POS Vừa Hoạt Động (Recent Connected Activity) -->
    <div class="p-5 rounded-3xl bg-slate-900 border border-slate-800 space-y-4">
      <div class="flex items-center justify-between border-b border-slate-800/80 pb-3">
        <div class="flex items-center gap-2">
          <Activity class="w-4 h-4 text-emerald-400" />
          <h3 class="font-bold text-white text-sm">Các Máy POS Vừa Đồng Bộ Gần Nhất</h3>
        </div>
        <span class="text-xs text-slate-400 font-mono">Tự động nhận diện qua Heartbeat</span>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 text-xs">
        <div 
          v-for="c in recentActiveCustomers" 
          :key="c.id"
          class="p-3 rounded-2xl bg-slate-950 border border-slate-800/80 flex flex-col justify-between"
        >
          <div class="space-y-1">
            <div class="flex items-center justify-between">
              <span class="font-bold text-white text-xs truncate max-w-[140px]">{{ c.shopName }}</span>
              <span 
                class="w-2 h-2 rounded-full"
                :class="isOnline(c) ? 'bg-emerald-400 animate-pulse' : 'bg-slate-600'"
              ></span>
            </div>
            <p class="text-[10px] text-slate-400 font-mono">{{ c.shopCode }} • {{ c.businessModel || 'Tiệm' }}</p>
          </div>
          <div class="mt-2 pt-2 border-t border-slate-800/60 flex items-center justify-between text-[10px] text-slate-500">
            <span>Ping cuối:</span>
            <span class="font-mono text-slate-300">{{ formatLastPing(c) }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL CHI TIẾT NHÓM KHÁCH HÀNG TRONG THÁNG (COHORT DETAIL MODAL) -->
    <div 
      v-if="activeCohortModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-xs animate-fade-in"
    >
      <div class="bg-slate-900 border border-slate-800 rounded-3xl max-w-2xl w-full p-6 space-y-4 shadow-2xl text-slate-200 animate-scale-up max-h-[85vh] flex flex-col">
        <!-- Header Modal -->
        <div class="flex items-center justify-between border-b border-slate-800 pb-3.5 shrink-0">
          <div class="flex items-center gap-2.5">
            <div 
              class="w-9 h-9 rounded-xl border flex items-center justify-center"
              :class="cohortModalHeaderClass.iconBg"
            >
              <component :is="cohortModalHeaderClass.icon" class="w-4 h-4" />
            </div>
            <div>
              <h3 class="font-extrabold text-white text-sm">
                {{ cohortModalHeaderClass.title }} (Tháng {{ String(selectedMonth).padStart(2, '0') }}/{{ selectedYear }})
              </h3>
              <p class="text-[10px] text-slate-400">
                {{ cohortModalHeaderClass.subtitle }}
              </p>
            </div>
          </div>
          <button @click="closeCohortList" class="text-slate-500 hover:text-white p-1 cursor-pointer">✕</button>
        </div>

        <!-- Danh Sách -->
        <div class="flex-1 overflow-y-auto min-h-48 border border-slate-800/80 rounded-2xl bg-slate-950/60">
          <!-- Khi rỗng -->
          <div v-if="cohortModalList.length === 0" class="py-12 text-center text-slate-500 text-xs">
            <p class="font-medium text-slate-400">Không có dữ liệu trong tháng này</p>
            <p class="text-[11px] text-slate-500 mt-0.5">Không phát sinh tiệm nào thuộc danh mục này trong Tháng {{ selectedMonth }}/{{ selectedYear }}.</p>
          </div>

          <!-- Bảng Khách Hàng Mới hoặc Khách Quá Hạn -->
          <table v-else-if="activeCohortModal !== 'renewed'" class="w-full text-left text-xs text-slate-300">
            <thead class="bg-slate-900 border-b border-slate-800 text-[10px] font-bold text-slate-400 uppercase tracking-wider sticky top-0">
              <tr>
                <th class="py-3 px-3.5">Tên Quán</th>
                <th class="py-3 px-3.5">Chủ Tiệm / SĐT</th>
                <th class="py-3 px-3.5">Gói Cước</th>
                <th class="py-3 px-3.5">
                  {{ activeCohortModal === 'new' ? 'Ngày Tạo' : 'Ngày Hết Hạn' }}
                </th>
                <th class="py-3 px-3.5 text-right">Thao Tác</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-800/60 font-medium">
              <tr 
                v-for="c in cohortModalList" 
                :key="c.id"
                class="hover:bg-slate-800/30 transition text-[11px]"
              >
                <td class="py-3 px-3.5">
                  <span class="font-bold text-white block">{{ c.shopName }}</span>
                  <span class="text-slate-500 font-mono text-[10px]">{{ c.shopCode }} • {{ c.businessModel || 'Tiệm' }}</span>
                </td>
                <td class="py-3 px-3.5">
                  <span class="text-slate-300 block font-medium">{{ c.ownerName || 'Chưa cập nhật' }}</span>
                  <div class="flex items-center gap-1.5 mt-0.5">
                    <a 
                      v-if="c.phone"
                      :href="`tel:${c.phone}`" 
                      class="text-indigo-400 hover:text-indigo-300 font-mono text-[10px] flex items-center gap-1"
                    >
                      <Phone class="w-3 h-3" />
                      <span>{{ c.phone }}</span>
                    </a>
                    <span v-else class="text-slate-600 text-[10px]">Chưa có SĐT</span>
                  </div>
                </td>
                <td class="py-3 px-3.5 whitespace-nowrap">
                  <span 
                    class="px-2 py-0.5 rounded text-[10px] font-extrabold uppercase border"
                    :class="getPlanBadgeClass(c.currentPlan)"
                  >
                    {{ c.currentPlan || 'Trial' }}
                  </span>
                </td>
                <td class="py-3 px-3.5 whitespace-nowrap font-mono text-slate-300">
                  {{ activeCohortModal === 'new' ? formatDate(c.createdAt) : formatDate(c.expiresAt) }}
                </td>
                <td class="py-3 px-3.5 text-right whitespace-nowrap">
                  <button
                    @click="$emit('open-license-modal', c); closeCohortList()"
                    class="px-2.5 py-1 rounded-lg bg-indigo-600/30 hover:bg-indigo-600/50 text-indigo-300 border border-indigo-500/30 font-bold text-[10px] transition cursor-pointer"
                  >
                    Gia Hạn ⚡
                  </button>
                </td>
              </tr>
            </tbody>
          </table>

          <!-- Bảng Danh Sách Lượt Gia Hạn -->
          <table v-else class="w-full text-left text-xs text-slate-300">
            <thead class="bg-slate-900 border-b border-slate-800 text-[10px] font-bold text-slate-400 uppercase tracking-wider sticky top-0">
              <tr>
                <th class="py-3 px-3.5">Thời Gian</th>
                <th class="py-3 px-3.5">Quán Gia Hạn</th>
                <th class="py-3 px-3.5">Gói Cấp</th>
                <th class="py-3 px-3.5">Số Tháng</th>
                <th class="py-3 px-3.5 text-right">Số Tiền Thu</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-800/60 font-medium">
              <tr 
                v-for="r in cohortModalList" 
                :key="r.id || r.issuedAt"
                class="hover:bg-slate-800/30 transition text-[11px]"
              >
                <td class="py-3 px-3.5 whitespace-nowrap text-slate-400 font-mono">
                  {{ formatDate(r.issuedAt) }}
                </td>
                <td class="py-3 px-3.5">
                  <span class="font-bold text-white block">{{ r.shopName }}</span>
                  <span class="text-slate-500 font-mono text-[10px]">({{ r.shopCode }})</span>
                </td>
                <td class="py-3 px-3.5 whitespace-nowrap">
                  <span 
                    class="px-2 py-0.5 rounded text-[10px] font-extrabold uppercase border"
                    :class="getPlanBadgeClass(r.planType)"
                  >
                    {{ r.planType }}
                  </span>
                </td>
                <td class="py-3 px-3.5 whitespace-nowrap text-slate-300 font-bold">
                  +{{ r.months }} tháng
                </td>
                <td class="py-3 px-3.5 text-right whitespace-nowrap font-black text-emerald-400 font-mono">
                  {{ formatCurrency(r.price) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Footer Modal -->
        <div class="flex items-center justify-between pt-2 border-t border-slate-800 shrink-0">
          <span class="text-[11px] text-slate-500">
            Tổng cộng: <b class="text-white">{{ cohortModalList.length }}</b> mục
          </span>
          <button
            @click="closeCohortList"
            class="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-200 font-bold text-xs cursor-pointer"
          >
            Đóng
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import {
  Store,
  CheckCircle2,
  AlertTriangle,
  DollarSign,
  Activity,
  Clock,
  TrendingUp,
  ShieldCheck,
  Plus,
  RefreshCw,
  Key,
  ArrowUpRight,
  Calendar,
  ChevronLeft,
  ChevronRight,
  UserPlus,
  Repeat,
  UserMinus,
  Phone
} from 'lucide-vue-next'

const props = defineProps({
  customers: {
    type: Array,
    default: () => []
  },
  records: {
    type: Array,
    default: () => []
  },
  revenue: {
    type: Number,
    default: 0
  },
  loading: Boolean
})

defineEmits([
  'refresh',
  'open-new-customer',
  'switch-tab',
  'quick-extend-1m',
  'quick-extend-1y',
  'open-license-modal',
  'open-history'
])

// Tính toán các số liệu
const now = new Date()

const activeCount = computed(() => {
  return props.customers.filter(c => {
    const exp = new Date(c.expiresAt)
    return c.status === 'Active' && exp > now
  }).length
})

const activePercent = computed(() => {
  if (props.customers.length === 0) return 0
  return Math.round((activeCount.value / props.customers.length) * 100)
})

const onlineCount = computed(() => {
  return props.customers.filter(c => isOnline(c)).length
})

const expiringSoonCount = computed(() => {
  const soon = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000)
  return props.customers.filter(c => {
    const exp = new Date(c.expiresAt)
    return c.status === 'Active' && exp > now && exp <= soon
  }).length
})

const expiredCount = computed(() => {
  return props.customers.filter(c => {
    const exp = new Date(c.expiresAt)
    return exp <= now || c.status === 'Suspended'
  }).length
})

const suspendedOrExpiredCount = computed(() => {
  return props.customers.filter(c => {
    const exp = new Date(c.expiresAt)
    return c.status === 'Suspended' || exp <= now
  }).length
})

// Doanh thu tính toán từ lịch sử gia hạn thực tế
const displayRevenue = computed(() => {
  if (props.records && props.records.length > 0) {
    return props.records.reduce((sum, r) => sum + (Number(r.price) || 0), 0)
  }
  if (props.revenue > 0) return props.revenue
  // Fallback tạm theo số tiệm active nếu chưa có lịch sử
  return props.customers.reduce((sum, c) => {
    switch (c.currentPlan) {
      case 'Lifetime': return sum + 2000000
      case 'Yearly': return sum + 990000
      case 'Monthly': return sum + 99000
      default: return sum + 0
    }
  }, 0)
})

const estimatedMRR = computed(() => {
  // Doanh thu định kỳ hàng tháng (chuẩn 99k/tháng)
  const monthly = props.customers.filter(c => c.currentPlan === 'Monthly').length * 99000
  const yearlyProrated = props.customers.filter(c => c.currentPlan === 'Yearly').length * (990000 / 12)
  return Math.round(monthly + yearlyProrated)
})

// Phân bổ gói cước
const planDistribution = computed(() => {
  const total = props.customers.length || 1
  const map = {
    Trial: 0,
    Monthly: 0,
    Yearly: 0,
    Lifetime: 0
  }

  props.customers.forEach(c => {
    const p = c.currentPlan || 'Trial'
    if (map[p] !== undefined) map[p]++
    else map.Trial++
  })

  return {
    Trial: { count: map.Trial, percent: Math.round((map.Trial / total) * 100) },
    Monthly: { count: map.Monthly, percent: Math.round((map.Monthly / total) * 100) },
    Yearly: { count: map.Yearly, percent: Math.round((map.Yearly / total) * 100) },
    Lifetime: { count: map.Lifetime, percent: Math.round((map.Lifetime / total) * 100) }
  }
})

// Phân bổ mô hình kinh doanh
const businessModelStats = computed(() => {
  const total = props.customers.length || 1
  const counts = {
    Barber: 0,
    Salon: 0,
    Spa: 0,
    Retail: 0,
    Other: 0
  }

  props.customers.forEach(c => {
    const m = c.businessModel || 'Barber'
    if (counts[m] !== undefined) counts[m]++
    else counts.Other++
  })

  return [
    { name: 'Barber (Tóc Nam)', icon: '✂️', count: counts.Barber, percent: Math.round((counts.Barber / total) * 100), color: 'bg-indigo-500' },
    { name: 'Salon Tóc Nữ', icon: '💇‍♀️', count: counts.Salon, percent: Math.round((counts.Salon / total) * 100), color: 'bg-rose-500' },
    { name: 'Nail & Spa Thẩm Mỹ', icon: '💅', count: counts.Spa, percent: Math.round((counts.Spa / total) * 100), color: 'bg-emerald-500' },
    { name: 'Bán Lẻ / Cửa Hàng', icon: '🛍️', count: counts.Retail, percent: Math.round((counts.Retail / total) * 100), color: 'bg-blue-500' },
    { name: 'Mô Hình Khác', icon: '☕', count: counts.Other, percent: Math.round((counts.Other / total) * 100), color: 'bg-slate-500' }
  ]
})

// Danh sách quán khẩn cấp cần gia hạn (hết hạn trong 7 ngày hoặc đã quá hạn)
const urgentCustomers = computed(() => {
  const soon = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000)
  return props.customers
    .filter(c => {
      const exp = new Date(c.expiresAt)
      return exp <= soon || c.status === 'Suspended'
    })
    .sort((a, b) => new Date(a.expiresAt) - new Date(b.expiresAt))
    .slice(0, 5) // Top 5 quán gấp nhất
})

// Danh sách quán vừa hoạt động
const recentActiveCustomers = computed(() => {
  return [...props.customers]
    .filter(c => c.lastPingAt)
    .sort((a, b) => new Date(b.lastPingAt) - new Date(a.lastPingAt))
    .slice(0, 4)
})

// Helper functions
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
  if (c.status === 'Suspended') return 'text-rose-400'
  const days = getDaysRemaining(c)
  if (days <= 0) return 'text-rose-400'
  if (days <= 7) return 'text-amber-400'
  return 'text-slate-400'
}

function getPlanBadgeClass(plan) {
  switch (plan) {
    case 'Lifetime': return 'bg-purple-500/20 text-purple-300 border-purple-500/30'
    case 'Yearly': return 'bg-indigo-500/20 text-indigo-300 border-indigo-500/30'
    case 'Monthly': return 'bg-blue-500/20 text-blue-300 border-blue-500/30'
    default: return 'bg-slate-800 text-slate-400 border-slate-700'
  }
}

// ==========================================
// LOGIC THEO DÕI TĂNG TRƯỞNG THEO THÁNG (COHORT)
// ==========================================
const selectedYear = ref(new Date().getFullYear())
const selectedMonth = ref(new Date().getMonth() + 1) // 1 - 12
const activeCohortModal = ref(null) // 'new' | 'renewed' | 'churned' | null

function prevMonth() {
  if (selectedMonth.value === 1) {
    selectedMonth.value = 12
    selectedYear.value--
  } else {
    selectedMonth.value--
  }
}

function nextMonth() {
  if (selectedMonth.value === 12) {
    selectedMonth.value = 1
    selectedYear.value++
  } else {
    selectedMonth.value++
  }
}

function resetToCurrentMonth() {
  const d = new Date()
  selectedYear.value = d.getFullYear()
  selectedMonth.value = d.getMonth() + 1
}

const isCurrentMonthSelected = computed(() => {
  const d = new Date()
  return selectedYear.value === d.getFullYear() && selectedMonth.value === (d.getMonth() + 1)
})

// Khoảng thời gian từ đầu tháng đến cuối tháng đang chọn
const monthRange = computed(() => {
  const y = selectedYear.value
  const m = selectedMonth.value
  const start = new Date(y, m - 1, 1, 0, 0, 0, 0)
  const end = new Date(y, m, 0, 23, 59, 59, 999)
  return { start, end }
})

// 1. Khách hàng mới tạo trong tháng được chọn
const newCustomersInMonth = computed(() => {
  const { start, end } = monthRange.value
  return props.customers.filter(c => {
    const raw = c.createdAt || c.created_at
    if (!raw) return false
    const d = new Date(raw)
    return d >= start && d <= end
  })
})

// 2. Lượt gia hạn phát sinh trong tháng được chọn
const renewalsInMonth = computed(() => {
  const { start, end } = monthRange.value
  return (props.records || []).filter(r => {
    const raw = r.issuedAt || r.issued_at
    if (!raw) return false
    const d = new Date(raw)
    return d >= start && d <= end
  })
})

// Tổng doanh thu thu được từ các lần gia hạn trong tháng
const renewalRevenueInMonth = computed(() => {
  return renewalsInMonth.value.reduce((sum, r) => sum + (Number(r.price) || 0), 0)
})

// 3. Khách bỏ / quá hạn (Hết hạn trong tháng này nhưng không gia hạn tiếp)
const churnedCustomersInMonth = computed(() => {
  const { start, end } = monthRange.value
  return props.customers.filter(c => {
    const rawExp = c.expiresAt || c.expires_at
    if (!rawExp) return false
    const exp = new Date(rawExp)
    // Hết hạn trong tháng đang chọn
    const isExpiredInMonth = exp >= start && exp <= end
    // Và hiện tại vẫn ở trạng thái quá hạn hoặc bị khóa
    const isStillExpired = exp <= now || c.status === 'Suspended' || c.status === 'Expired'
    return isExpiredInMonth && isStillExpired
  })
})

// 4. Tăng trưởng ròng (Net Growth) = Khách Mới - Khách Bỏ
const netGrowth = computed(() => {
  return newCustomersInMonth.value.length - churnedCustomersInMonth.value.length
})

// Dữ liệu cho Modal chi tiết
function openCohortList(type) {
  activeCohortModal.value = type
}

function closeCohortList() {
  activeCohortModal.value = null
}

const cohortModalList = computed(() => {
  switch (activeCohortModal.value) {
    case 'new': return newCustomersInMonth.value
    case 'renewed': return renewalsInMonth.value
    case 'churned': return churnedCustomersInMonth.value
    default: return []
  }
})

const cohortModalHeaderClass = computed(() => {
  switch (activeCohortModal.value) {
    case 'new':
      return {
        title: 'Danh Sách Khách Hàng Mới',
        subtitle: `Các tiệm mới đăng ký & kích hoạt trong tháng ${selectedMonth.value}/${selectedYear.value}`,
        icon: UserPlus,
        iconBg: 'bg-emerald-500/20 text-emerald-300 border-emerald-500/30'
      }
    case 'renewed':
      return {
        title: 'Danh Sách Lượt Gia Hạn Bản Quyền',
        subtitle: `Tất cả các giao dịch đóng phí bản quyền trong tháng ${selectedMonth.value}/${selectedYear.value}`,
        icon: Repeat,
        iconBg: 'bg-blue-500/20 text-blue-300 border-blue-500/30'
      }
    case 'churned':
      return {
        title: 'Danh Sách Tiệm Quá Hạn Cần Chăm Sóc Lại',
        subtitle: `Các quán hết hạn trong tháng ${selectedMonth.value}/${selectedYear.value} chưa đóng tiền tiếp`,
        icon: UserMinus,
        iconBg: 'bg-rose-500/20 text-rose-300 border-rose-500/30'
      }
    default:
      return {
        title: 'Chi Tiết Nhóm Khách Hàng',
        subtitle: '',
        icon: Calendar,
        iconBg: 'bg-slate-800 text-slate-300 border-slate-700'
      }
  }
})
</script>
