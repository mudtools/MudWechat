import { computed, onMounted, ref } from 'vue'
import { getHealth } from '../api/endpoints'
import type { HealthInfo } from '../api/types'
import { presentError } from '../utils/notify'

/**
 * 全局共享的健康信息（**不引 Pinia**：一个 ref 足矣，方案 §7.1 末条）。
 *
 * `writeGate` 是写按钮置灰的唯一依据 —— 闸关时后端根本不映射写路由，
 * 前端靠状态码兜底会得到 404 或 405 这种「看起来像 bug」的反馈（方案 §7.3）。
 */
const health = ref<HealthInfo | null>(null)
const loading = ref(false)

export function useHealth() {
  const writeEnabled = computed(() => health.value?.writeGate === 'enabled')

  async function refresh(): Promise<void> {
    loading.value = true
    try {
      health.value = await getHealth()
    } catch (reason) {
      presentError(reason)
    } finally {
      loading.value = false
    }
  }

  onMounted(() => {
    if (health.value === null) {
      void refresh()
    }
  })

  return { health, writeEnabled, loading, refresh }
}
