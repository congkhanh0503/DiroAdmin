<template>
  <div 
    v-if="isOpen" 
    class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-xs animate-fade-in"
  >
    <div class="bg-slate-900 border border-slate-800 rounded-3xl max-w-xl w-full p-6 space-y-4 shadow-2xl text-slate-200 animate-scale-up flex flex-col max-h-[85vh]">
      <!-- Modal Header -->
      <div class="flex items-center justify-between border-b border-slate-800 pb-3.5 shrink-0">
        <div class="flex items-center gap-2.5">
          <div class="w-8 h-8 rounded-xl bg-indigo-600/20 border border-indigo-500/30 flex items-center justify-center text-indigo-400">
            <Cloud class="w-4 h-4" />
          </div>
          <div>
            <h3 class="font-extrabold text-white text-sm">
              Bản Sao Lưu Cloud: {{ customer?.shopName || 'Khách Hàng' }}
            </h3>
            <p class="text-[10px] text-slate-400">
              Mã Quán: <b class="text-indigo-400 font-mono">{{ customer?.shopCode }}</b> • Tự động lưu 3 bản mới nhất trên Cloud
            </p>
          </div>
        </div>
        <button @click="$emit('close')" class="text-slate-500 hover:text-white p-1 cursor-pointer">✕</button>
      </div>

      <!-- Quick Info / Status Bar -->
      <div class="p-3 rounded-2xl bg-slate-950 border border-slate-800 flex items-center justify-between text-xs shrink-0">
        <div class="flex items-center gap-2">
          <span class="w-2 h-2 rounded-full bg-emerald-400 animate-pulse"></span>
          <span class="text-slate-300">Dung lượng: <b>{{ backups.length }} / 3 bản sao lưu</b></span>
        </div>
        <button
          @click="fetchBackups"
          :disabled="loading"
          class="text-indigo-400 hover:text-indigo-300 text-[11px] font-semibold flex items-center gap-1 cursor-pointer"
        >
          <RefreshCw class="w-3 h-3" :class="{ 'animate-spin': loading }" />
          <span>Làm mới</span>
        </button>
      </div>

      <!-- Backups List -->
      <div class="flex-1 overflow-y-auto min-h-48 border border-slate-800/80 rounded-2xl bg-slate-950/60 p-3 space-y-2.5">
        <div v-if="loading" class="py-12 text-center text-slate-500 text-xs flex flex-col items-center gap-2">
          <RefreshCw class="w-6 h-6 animate-spin text-indigo-500" />
          <p>Đang tải danh sách bản sao lưu từ Cloud...</p>
        </div>

        <div v-else-if="backups.length === 0" class="py-10 text-center text-slate-500 text-xs">
          <Cloud class="w-8 h-8 mx-auto mb-2 text-slate-600 stroke-1" />
          <p class="font-medium text-slate-400">Chưa có bản sao lưu nào trên Đám mây</p>
          <p class="text-[11px] text-slate-500 mt-1 max-w-sm mx-auto">
            Khi quán bấm <b>"Đóng Ca"</b> hoặc bấm <b>"Sao Lưu Lên Cloud Ngay"</b> trên máy POS, file dữ liệu sẽ tự động đồng bộ lên đây.
          </p>
        </div>

        <div v-else class="space-y-2">
          <div
            v-for="(b, idx) in backups"
            :key="b.name"
            class="flex items-center justify-between p-3.5 rounded-xl bg-slate-900 border border-slate-800/90 hover:border-slate-700 transition"
          >
            <div class="flex items-center gap-3">
              <span 
                class="w-7 h-7 rounded-lg flex items-center justify-center font-bold text-xs"
                :class="idx === 0 ? 'bg-emerald-500/20 text-emerald-400 border border-emerald-500/30' : 'bg-slate-800 text-slate-400'"
              >
                #{{ idx + 1 }}
              </span>
              <div>
                <div class="flex items-center gap-2">
                  <p class="font-bold text-white font-mono text-xs">{{ b.name }}</p>
                  <span v-if="idx === 0" class="px-1.5 py-0.2 rounded text-[9px] font-black bg-emerald-500/20 text-emerald-300 border border-emerald-500/30">
                    MỚI NHẤT
                  </span>
                </div>
                <p class="text-[10px] text-slate-400 mt-0.5">
                  Thời gian: <b class="text-slate-300">{{ formatDateTime(b.createdAt) }}</b> • Nén: <b class="text-emerald-400">{{ b.sizeFormatted }}</b>
                </p>
              </div>
            </div>

            <a
              :href="b.downloadUrl"
              target="_blank"
              class="px-3 py-1.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white font-bold text-xs shadow-xs transition flex items-center gap-1.5 cursor-pointer active:scale-95 whitespace-nowrap"
              title="Tải file zip về máy tính của bạn"
            >
              <Download class="w-3.5 h-3.5" />
              <span>Tải Về Máy</span>
            </a>
          </div>
        </div>
      </div>

      <!-- Modal Footer -->
      <div class="flex items-center justify-between pt-2 border-t border-slate-800 shrink-0">
        <p class="text-[11px] text-slate-500">
          💡 File backup là dạng nén .zip chứa <code class="text-indigo-300">diropos.db</code>.
        </p>
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
import { ref, watch } from 'vue'
import api from '../api'
import { Cloud, Download, RefreshCw } from 'lucide-vue-next'

const props = defineProps({
  isOpen: Boolean,
  customer: {
    type: Object,
    default: null
  }
})

defineEmits(['close'])

const backups = ref([])
const loading = ref(false)

async function fetchBackups() {
  if (!props.customer?.shopCode) return
  loading.value = true
  try {
    const list = await api.getCustomerBackups(props.customer.shopCode)
    backups.value = list
  } catch (err) {
    console.error('Lỗi tải backup:', err)
  } finally {
    loading.value = false
  }
}

watch(() => props.isOpen, (newVal) => {
  if (newVal && props.customer) {
    fetchBackups()
  } else {
    backups.value = []
  }
})

function formatDateTime(isoStr) {
  if (!isoStr) return '---'
  try {
    const d = new Date(isoStr)
    const hh = String(d.getHours()).padStart(2, '0')
    const mm = String(d.getMinutes()).padStart(2, '0')
    const day = String(d.getDate()).padStart(2, '0')
    const month = String(d.getMonth() + 1).padStart(2, '0')
    const year = d.getFullYear()
    return `${hh}:${mm} - ${day}/${month}/${year}`
  } catch {
    return isoStr
  }
}
</script>
