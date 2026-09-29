import { createClient } from '@supabase/supabase-js'

export const SUPABASE_URL = 'https://lacdvcuztjafnwpitntk.supabase.co'
export const SUPABASE_KEY = 'sb_publishable_myN8hP6gC-sj8jhX9OAIrQ_Q1_F94So'

export const supabase = createClient(SUPABASE_URL, SUPABASE_KEY, {
  auth: {
    persistSession: true,
    autoRefreshToken: true
  }
})

export default supabase
