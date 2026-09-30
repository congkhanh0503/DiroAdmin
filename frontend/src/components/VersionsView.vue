<template>
  <div class="space-y-6">
    <!-- Header Banner -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 p-6 rounded-3xl bg-slate-900 border border-slate-800 shadow-xl">
      <div>
        <div class="flex items-center gap-2.5">
          <div class="w-10 h-10 rounded-2xl bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center text-white shadow-lg shadow-indigo-500/25">
            <Rocket class="w-5 h-5" />
          </div>
          <div>
            <h2 class="text-xl font-black text-white tracking-tight">Trung Tâm Quản Lý Phiên Bản DiroPos</h2>
            <p class="text-xs text-slate-400 mt-0.5">Phát hành bản cập nhật, tải file cài đặt và quản lý changelog cho toàn bộ máy khách</p>
          </div>
        </div>
      </div>

      <div class="flex items-center gap-3">
        <button
          type="button"
          @click="loadVersions"
          :disabled="loading"
          class="p-2.5 rounded-xl border border-slate-800 text-slate-400 hover:text-white hover:bg-slate-800 transition cursor-pointer"
          title="Tải lại danh sách"
        >
          <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': loading }" />
        </button>

        <button
          type="button"
          @click="openReleaseModal"
          class="px-4 py-2.5 rounded-xl bg-gradient-to-r from-indigo-600 to-blue-600 hover:from-indigo-500 hover:to-blue-500 text-white font-bold text-xs shadow-md shadow-indigo-600/30 transition flex items-center gap-2 cursor-pointer active:scale-95"
        >
          <Plus class="w-4 h-4" />
          <span>Phát Hành Bản Mới</span>
        </button>
      </div>
    </div>

    <!-- Quick Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
      <div class="p-5 rounded-2xl bg-slate-900/80 border border-slate-800/80 flex items-center justify-between">
        <div>
          <p class="text-[11px] font-bold text-slate-400 uppercase tracking-wider">Phiên Bản Mới Nhất</p>
          <h3 class="text-2xl font-black text-indigo-400 mt-1">
            {{ latestVersion ? 'v' + latestVersion.version : 'Chưa có' }}
          </h3>
          <p class="text-[10px] text-slate-500 mt-0.5">
            {{ latestVersion ? formatDate(latestVersion.release_date) : '---' }}
          </p>
        </div>
        <div class="w-12 h-12 rounded-2xl bg-indigo-500/10 border border-indigo-500/20 flex items-center justify-center text-indigo-400">
          <Sparkles class="w-6 h-6" />
        </div>
      </div>

      <div class="p-5 rounded-2xl bg-slate-900/80 border border-slate-800/80 flex items-center justify-between">
        <div>
          <p class="text-[11px] font-bold text-slate-400 uppercase tracking-wider">Tổng Số Bản Đã Phát Hành</p>
          <h3 class="text-2xl font-black text-emerald-400 mt-1">
            {{ versions.length }}
          </h3>
          <p class="text-[10px] text-slate-500 mt-0.5">Lưu trữ trên Supabase Cloud</p>
        </div>
        <div class="w-12 h-12 rounded-2xl bg-emerald-500/10 border border-emerald-500/20 flex items-center justify-center text-emerald-400">
          <Layers class="w-6 h-6" />
        </div>
      </div>

      <div class="p-5 rounded-2xl bg-slate-900/80 border border-slate-800/80 flex items-center justify-between">
        <div>
          <p class="text-[11px] font-bold text-slate-400 uppercase tracking-wider">Cơ Chế Phân Phối</p>
          <h3 class="text-base font-bold text-slate-200 mt-1">Zero-Touch OTA</h3>
          <p class="text-[10px] text-slate-500 mt-0.5">Máy POS tự động phát hiện khi khởi động</p>
        </div>
        <div class="w-12 h-12 rounded-2xl bg-purple-500/10 border border-purple-500/20 flex items-center justify-center text-purple-400">
          <CheckCircle2 class="w-6 h-6" />
        </div>
      </div>
    </div>

    <!-- Table of Versions -->
    <div class="rounded-3xl bg-slate-900 border border-slate-800 overflow-hidden shadow-xl">
      <div class="p-5 border-b border-slate-800 flex items-center justify-between">
        <div class="flex items-center gap-2">
          <h3 class="font-bold text-white text-sm">Lịch Sử Các Bản Phát Hành</h3>
          <span class="px-2 py-0.5 rounded-full text-[10px] font-bold bg-slate-800 text-slate-400">
            {{ versions.length }} bản
          </span>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="py-16 text-center text-slate-500 flex flex-col items-center gap-3">
        <RefreshCw class="w-6 h-6 animate-spin text-indigo-500" />
        <p class="text-xs">Đang tải danh sách phiên bản từ Supabase...</p>
      </div>

      <!-- Empty State -->
      <div v-else-if="versions.length === 0" class="py-16 text-center text-slate-500 px-4">
        <Rocket class="w-10 h-10 mx-auto mb-2 text-slate-600 stroke-1" />
        <p class="text-sm font-bold text-slate-400">Chưa có phiên bản nào được lưu trên Cloud</p>
        <p class="text-xs text-slate-600 mt-1 max-w-sm mx-auto">
          Hãy bấm "Phát Hành Bản Mới" để tạo bản v1.0.0 chính thức đầu tiên.
        </p>
        <button
          type="button"
          @click="openReleaseModal"
          class="mt-4 px-4 py-2 rounded-xl bg-indigo-600 text-white text-xs font-bold hover:bg-indigo-500 transition cursor-pointer"
        >
          Tạo Phiên Bản Đầu Tiên
        </button>
      </div>

      <!-- Version List -->
      <div v-else class="divide-y divide-slate-800">
        <div
          v-for="(v, index) in versions"
          :key="v.id || v.version"
          class="p-5 hover:bg-slate-800/40 transition flex flex-col lg:flex-row lg:items-start justify-between gap-4"
        >
          <!-- Left Info -->
          <div class="space-y-2 flex-1 min-w-0">
            <div class="flex items-center gap-3 flex-wrap">
              <span class="font-mono font-black text-base text-indigo-400">
                v{{ v.version }}
              </span>

              <span
                v-if="index === 0"
                class="px-2 py-0.5 rounded text-[10px] font-black bg-indigo-500/20 text-indigo-300 border border-indigo-500/30 uppercase tracking-wider"
              >
                Mới Nhất
              </span>

              <span
                class="px-2 py-0.5 rounded text-[10px] font-extrabold uppercase tracking-wider"
                :class="v.is_mandatory ? 'bg-rose-500/20 text-rose-400 border border-rose-500/30' : 'bg-emerald-500/20 text-emerald-400 border border-emerald-500/30'"
              >
                {{ v.is_mandatory ? 'Bắt Buộc Cập Nhật' : 'Cập Nhật Tùy Chọn' }}
              </span>

              <span class="text-xs text-slate-500">
                Ngày phát hành: <b class="text-slate-300">{{ formatDate(v.release_date) }}</b>
              </span>
            </div>

            <!-- Changelog -->
            <div class="p-3.5 rounded-xl bg-slate-950 border border-slate-800/80 text-xs text-slate-300 font-medium whitespace-pre-line leading-relaxed">
              {{ v.changelog || 'Không có mô tả chi tiết.' }}
            </div>

            <!-- Download URL -->
            <div v-if="v.download_url" class="flex items-center gap-2 text-xs">
              <span class="text-slate-500">Liên kết tải:</span>
              <a
                :href="v.download_url"
                target="_blank"
                class="text-indigo-400 hover:text-indigo-300 hover:underline truncate max-w-md font-mono"
              >
                {{ v.download_url }}
              </a>
            </div>
          </div>

          <!-- Right Actions -->
          <div class="flex items-center gap-2 self-end lg:self-start shrink-0">
            <a
              v-if="v.download_url"
              :href="v.download_url"
              target="_blank"
              class="px-3 py-1.5 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-200 text-xs font-semibold transition flex items-center gap-1.5 cursor-pointer"
            >
              <Download class="w-3.5 h-3.5" />
              <span>Tải Thử</span>
            </a>

            <button
              type="button"
              @click="handleDelete(v)"
              class="p-2 rounded-xl text-slate-500 hover:text-rose-400 hover:bg-rose-500/10 transition cursor-pointer"
              title="Xóa phiên bản này"
            >
              <Trash2 class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL: Phát Hành Bản Cập Nhật Mới -->
    <div
      v-if="showModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/75 backdrop-blur-xs animate-fade-in"
      @click.self="showModal = false"
    >
      <div class="bg-slate-900 border border-slate-800 rounded-3xl max-w-lg w-full p-6 space-y-4 shadow-2xl text-slate-200 animate-scale-up">
        <!-- Header -->
        <div class="flex items-center justify-between border-b border-slate-800 pb-3">
          <div class="flex items-center gap-2.5">
            <div class="w-8 h-8 rounded-xl bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center text-white">
              <Rocket class="w-4 h-4" />
            </div>
            <div>
              <h3 class="font-black text-white text-sm">Phát Hành Phiên Bản DiroPos Mới</h3>
              <p class="text-[10px] text-slate-400">Máy khách sẽ tự động nhận diện và hiện thông báo tải bản mới này</p>
            </div>
          </div>
          <button @click="showModal = false" class="text-slate-500 hover:text-white p-1 cursor-pointer">✕</button>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSubmit" class="space-y-3.5 text-xs">
          <div>
            <label class="block text-slate-300 mb-1 font-medium">Số Phiên Bản (Version) *</label>
            <input
              v-model="form.version"
              type="text"
              required
              placeholder="VD: 1.1.0"
              class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3.5 py-2 text-white font-mono text-xs focus:outline-none focus:border-indigo-500"
            />
          </div>

          <div>
            <label class="block text-slate-300 mb-1 font-medium">Đường Dẫn Tải File Cài Đặt (.exe) *</label>
            <input
              v-model="form.download_url"
              type="url"
              required
              placeholder="https://github.com/.../DiroPos_Setup_v1.1.0.exe"
              class="w-full bg-slate-950 border border-slate-800 rounded-xl px-3.5 py-2 text-white font-mono text-xs focus:outline-none focus:border-indigo-500"
            />
            <p class="text-[10px] text-slate-500 mt-1">Khuyến nghị tải file .exe lên GitHub Releases hoặc Google Drive chia sẻ công khai.</p>
          </div>

          <div>
            <label class="block text-slate-300 mb-1 font-medium">Nội Dung Cập Nhật (Changelog) *</label>
            <textarea
              v-model="form.changelog"
              required
              rows="4"
              placeholder="• Thêm tính năng in hóa đơn 80mm tự động&#10;• Cải thiện tốc độ tạo mã VietQR&#10;• Sửa lỗi hiển thị doanh thu ca tối"
              class="w-full bg-slate-950 border border-slate-800 rounded-xl p-3 text-white text-xs focus:outline-none focus:border-indigo-500 resize-none leading-relaxed"
            ></textarea>
          </div>

          <div class="p-3.5 rounded-xl bg-slate-950 border border-slate-800 space-y-2">
            <label class="flex items-center gap-2 cursor-pointer">
              <input
                v-model="form.is_mandatory"
                type="checkbox"
                class="w-4 h-4 rounded text-indigo-600 focus:ring-indigo-500 bg-slate-900 border-slate-700"
              />
              <span class="font-bold text-slate-200">Bắt buộc cập nhật (Mandatory)</span>
            </label>
            <p class="text-[10px] text-slate-400 pl-6">
              Nếu tích chọn, các máy khách chạy phiên bản cũ sẽ bắt buộc phải cập nhật mới được sử dụng tiếp.
            </p>
          </div>

          <!-- Buttons -->
          <div class="flex items-center justify-end gap-2 pt-3 border-t border-slate-800">
            <button
              type="button"
              @click="showModal = false"
              class="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 font-bold text-xs cursor-pointer"
            >
              Hủy
            </button>

            <button
              type="submit"
              :disabled="saving"
              class="px-5 py-2 rounded-xl bg-gradient-to-r from-indigo-600 to-purple-600 hover:from-indigo-500 hover:to-purple-500 text-white font-bold text-xs shadow-md shadow-indigo-600/30 transition flex items-center gap-1.5 cursor-pointer disabled:opacity-50"
            >
              <RefreshCw v-if="saving" class="w-3.5 h-3.5 animate-spin" />
              <span>{{ saving ? 'Đang phát hành...' : 'Phát Hành Ngay 🚀' }}</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '../api'
import {
  Rocket,
  Plus,
  RefreshCw,
  Sparkles,
  Layers,
  CheckCircle2,
  Download,
  Trash2
} from 'lucide-vue-next'

const emit = defineEmits(['notify'])

const versions = ref([])
const loading = ref(false)
const showModal = ref(false)
const saving = ref(false)

const form = ref({
  version: '',
  download_url: '',
  changelog: '',
  is_mandatory: false,
  min_version: '1.0.0'
})

const latestVersion = computed(() => versions.value?.[0] || null)

function formatDate(isoStr) {
  if (!isoStr) return '---'
  try {
    const d = new Date(isoStr)
    return `${String(d.getDate()).padStart(2, '0')}/${String(d.getMonth() + 1).padStart(2, '0')}/${d.getFullYear()}`
  } catch {
    return isoStr
  }
}

async function loadVersions() {
  loading.value = true
  try {
    const list = await api.getAppVersions()
    versions.value = list || []
  } catch (err) {
    console.error('Lỗi tải danh sách phiên bản:', err)
  } finally {
    loading.value = false
  }
}

function openReleaseModal() {
  form.value = {
    version: '',
    download_url: '',
    changelog: '',
    is_mandatory: false,
    min_version: '1.0.0'
  }
  showModal.value = true
}

async function handleSubmit() {
  if (!form.value.version.trim() || !form.value.download_url.trim()) return
  saving.value = true

  try {
    await api.createAppVersion(form.value)
    showModal.value = false
    alert(`Đã phát hành thành công phiên bản v${form.value.version}!`)
    await loadVersions()
  } catch (err) {
    alert('Lỗi phát hành phiên bản: ' + (err.message || err))
  } finally {
    saving.value = false
  }
}

async function handleDelete(v) {
  if (!confirm(`Xác nhận xóa bản phát hành v${v.version}?`)) return
  try {
    await api.deleteAppVersion(v.id)
    alert(`Đã xóa bản v${v.version}!`)
    await loadVersions()
  } catch (err) {
    alert('Lỗi khi xóa phiên bản: ' + (err.message || err))
  }
}

onMounted(() => {
  loadVersions()
})
</script>
