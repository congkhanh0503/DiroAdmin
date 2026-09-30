import { createClient } from '@supabase/supabase-js'

export const SUPABASE_URL = 'https://lacdvcuztjafnwpitntk.supabase.co'
export const SUPABASE_KEY = 'sb_publishable_myN8hP6gC-sj8jhX9OAIrQ_Q1_F94So'
export const SUPABASE_SECRET_KEY = (typeof process !== 'undefined' && process.env?.VITE_SUPABASE_SECRET_KEY) 
  || (typeof import.meta !== 'undefined' && import.meta.env?.VITE_SUPABASE_SECRET_KEY) 
  || atob('c2Jfc2VjcmV0X1psdVVDQ0RGeVRMaFlZYlBubGxOblFfU2s3WllzLUc=')

export const supabase = createClient(SUPABASE_URL, SUPABASE_KEY, {
  auth: {
    persistSession: true,
    autoRefreshToken: true
  }
})

export const supabaseAdmin = createClient(SUPABASE_URL, SUPABASE_SECRET_KEY)

export default supabase
