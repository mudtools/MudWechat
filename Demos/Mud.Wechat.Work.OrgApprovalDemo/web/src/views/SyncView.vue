<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { ElMessageBox } from 'element-plus'
import { Refresh } from '@element-plus/icons-vue'

import { confirmMissing, getSyncDiff, getSyncRun, listChanges, listSyncRuns, startSync } from '../api/endpoints'
import type { ChangeLogRow, OrgDiff, SyncRunView } from '../api/types'
import { useHealth } from '../composables/useHealth'
import { formatUnix, runStatusText, runStatusType } from '../utils/format'
import { presentError, presentSuccess } from '../utils/notify'

const { writeEnabled } = useHealth()

const runs = ref<SyncRunView[]>([])
const changes = ref<ChangeLogRow[]>([])
const selectedRunId = ref<number | null>(null)
const diff = ref<OrgDiff | null>(null)
const diffError = ref('')
const loading = ref(false)

let timer: number | null = null

const activeRuns = computed(() => runs.value.filter((run) => run.status === 0 || run.status === 1))

const selectedRun = computed(() => runs.value.find((run) => run.id === selectedRunId.value) ?? null)

const progress = computed(() => {
  const run = selectedRun.value
  if (!run || run.total <= 0) {
    return 0
  }

  return Math.min(100, Math.round((run.processed / run.total) * 100))
})

async function loadRuns(): Promise<void> {
  try {
    const result = await listSyncRuns(30)
    runs.value = result.items
    if (selectedRunId.value === null && result.items.length > 0) {
      await selectRun(result.items[0].id)
    }
  } catch (reason) {
    presentError(reason)
  }
}

async function loadChanges(): Promise<void> {
  try {
    const result = await listChanges(50)
    changes.value = result.items
  } catch (reason) {
    presentError(reason)
  }
}

async function selectRun(id: number): Promise<void> {
  selectedRunId.value = id
  diffError.value = ''
  diff.value = null
  try {
    const run = await getSyncRun(id)
    if (run.hasDiff) {
      diff.value = await getSyncDiff(id)
    } else {
      diffError.value = '本轮还没有变更报告（未完成，或该任务种类不产出 diff）'
    }
  } catch (reason) {
    diffError.value = (reason as Error).message
  }
}

async function refreshAll(): Promise<void> {
  loading.value = true
  try {
    await loadRuns()
    await loadChanges()
  } finally {
    loading.value = false
  }
}

async function runSync(mode: 'full' | 'reconcile'): Promise<void> {
  try {
    const result = await startSync(mode)
    presentSuccess(`已投递任务 #${result.runId}`)
    await loadRuns()
    await selectRun(result.runId)
  } catch (reason) {
    presentError(reason)
  }
}

/** el-table 的 row-click 是 (row, column, event) —— 用方法收口，模板里不写 TS 标注。 */
function handleRowClick(row: SyncRunView): void {
  void selectRun(row.id)
}

async function confirmDelete(entityType: 'dept' | 'user', entityId: string, name: string | null): Promise<void> {
  if (selectedRunId.value === null) {
    return
  }

  try {
    await ElMessageBox.confirm(
      `仅删除**本地镜像行**（企微侧不动）：${entityType === 'dept' ? '部门' : '成员'} ${name ?? entityId}（${entityId}）。\n`
        + '确认前请到企业微信后台核实它确实已不存在 —— 若只是应用可见范围收窄，删掉本地行等于丢失历史。',
      '确认删除本地镜像',
      { type: 'warning', confirmButtonText: '已核实，删除本地行', cancelButtonText: '取消' },
    )
  } catch {
    return
  }

  try {
    await confirmMissing(selectedRunId.value, entityType, entityId)
    presentSuccess('本地镜像行已删除')
    await selectRun(selectedRunId.value)
  } catch (reason) {
    presentError(reason)
  }
}

onMounted(async () => {
  await refreshAll()
  // 1 秒轮询：只有存在运行中的任务时才继续轮询，空闲时不打后端。
  timer = window.setInterval(() => {
    if (activeRuns.value.length > 0) {
      void loadRuns()
    }
  }, 1000)
})

onUnmounted(() => {
  if (timer !== null) {
    window.clearInterval(timer)
    timer = null
  }
})
</script>

<template>
  <div class="panel" v-loading="loading">
    <div class="toolbar">
      <el-button size="small" :disabled="!writeEnabled" @click="runSync('full')">全量同步</el-button>
      <el-button size="small" :disabled="!writeEnabled" @click="runSync('reconcile')">对账（只读）</el-button>
      <span class="toolbar__spacer" />
      <el-tag v-if="activeRuns.length > 0" size="small" type="warning">有 {{ activeRuns.length }} 个任务在跑（1s 轮询）</el-tag>
      <el-button size="small" :icon="Refresh" @click="refreshAll">刷新</el-button>
    </div>

    <h3 class="panel__title">任务台账</h3>
    <el-table
      :data="runs"
      size="small"
      border
      highlight-current-row
      @row-click="handleRowClick"
    >
      <el-table-column prop="id" label="#" width="70" />
      <el-table-column prop="kind" label="种类" width="110" />
      <el-table-column label="状态" width="110">
        <template #default="{ row }">
          <el-tag size="small" :type="runStatusType(row.status)">{{ runStatusText(row.status) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="phase" label="阶段" width="130" />
      <el-table-column label="部门" width="80">
        <template #default="{ row }">{{ row.deptCount }}</template>
      </el-table-column>
      <el-table-column label="成员" width="80">
        <template #default="{ row }">{{ row.userCount }}</template>
      </el-table-column>
      <el-table-column label="开始" width="160">
        <template #default="{ row }">{{ formatUnix(row.startedAt) }}</template>
      </el-table-column>
      <el-table-column label="结束" width="160">
        <template #default="{ row }">{{ formatUnix(row.finishedAt) }}</template>
      </el-table-column>
      <el-table-column label="错误" min-width="200">
        <template #default="{ row }">
          <span v-if="row.errorMessage" class="dim">{{ row.errorCode }} {{ row.errorMessage }}</span>
          <span v-else class="dim">—</span>
        </template>
      </el-table-column>
    </el-table>

    <template v-if="selectedRun">
      <h3 class="panel__title" style="margin-top: 16px">
        任务 #{{ selectedRun.id }} 进度
        <span class="dim">（{{ selectedRun.phase ?? '—' }} · {{ selectedRun.processed }}/{{ selectedRun.total }}）</span>
      </h3>
      <el-progress :percentage="progress" :status="selectedRun.status === 3 ? 'exception' : undefined" />

      <h3 class="panel__title" style="margin-top: 16px">变更报告</h3>
      <el-alert v-if="diffError" type="info" :closable="false" :title="diffError" style="margin-bottom: 10px" />
      <el-alert
        v-if="diff?.guardian"
        type="warning"
        :closable="false"
        show-icon
        :title="diff.guardian"
        style="margin-bottom: 10px"
      />

      <el-tabs v-if="diff" type="border-card">
        <el-tab-pane :label="`新增 (${diff.created.length})`" name="created">
          <el-table :data="diff.created" size="small" border>
            <el-table-column prop="kind" label="类型" width="90">
              <template #default="{ row }">{{ row.kind === 'dept' ? '部门' : '成员' }}</template>
            </el-table-column>
            <el-table-column label="id" min-width="180">
              <template #default="{ row }"><span class="mono">{{ row.id }}</span></template>
            </el-table-column>
            <el-table-column prop="name" label="名称" min-width="160" />
          </el-table>
        </el-tab-pane>

        <el-tab-pane :label="`更新 (${diff.updated.length})`" name="updated">
          <el-table :data="diff.updated" size="small" border>
            <el-table-column prop="kind" label="类型" width="90">
              <template #default="{ row }">{{ row.kind === 'dept' ? '部门' : '成员' }}</template>
            </el-table-column>
            <el-table-column label="id" min-width="180">
              <template #default="{ row }"><span class="mono">{{ row.id }}</span></template>
            </el-table-column>
            <el-table-column prop="name" label="名称" min-width="160" />
          </el-table>
        </el-tab-pane>

        <el-tab-pane :label="`疑似删除 (${diff.missing.length})`" name="missing">
          <el-alert type="warning" :closable="false" show-icon title="「本轮未出现」不等于「已删除」" style="margin-bottom: 8px">
            <template #default>
              应用可见范围收窄、权限被改、某次 API 半途失败都会让本地记录凭空「消失」。
              同步只把它们标为「疑似删除」，<b>物理删除必须由这里人工确认</b>（且只删本地镜像行）。
            </template>
          </el-alert>
          <el-table :data="diff.missing" size="small" border>
            <el-table-column prop="kind" label="类型" width="90">
              <template #default="{ row }">{{ row.kind === 'dept' ? '部门' : '成员' }}</template>
            </el-table-column>
            <el-table-column label="id" min-width="180">
              <template #default="{ row }"><span class="mono">{{ row.id }}</span></template>
            </el-table-column>
            <el-table-column prop="name" label="名称" min-width="140" />
            <el-table-column label="操作" width="150" fixed="right">
              <template #default="{ row }">
                <el-button
                  link
                  size="small"
                  type="danger"
                  :disabled="!writeEnabled"
                  @click="confirmDelete(row.kind === 'dept' ? 'dept' : 'user', row.id, row.name)"
                >
                  确认删除本地行
                </el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
      </el-tabs>
    </template>

    <h3 class="panel__title" style="margin-top: 16px">最近回调（{{ changes.length }}）</h3>
    <el-table :data="changes" size="small" border>
      <el-table-column prop="id" label="#" width="70" />
      <el-table-column prop="source" label="来源" width="100" />
      <el-table-column label="对象" width="200">
        <template #default="{ row }">
          {{ row.entityType }} · <span class="mono">{{ row.entityId }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="kind" label="类别" width="100" />
      <el-table-column label="事件时间" width="170">
        <template #default="{ row }">{{ formatUnix(row.occurredAt) }}</template>
      </el-table-column>
      <el-table-column label="已应用" width="100">
        <template #default="{ row }">
          <el-tag size="small" :type="row.applied === 1 ? 'success' : 'info'">
            {{ row.applied === 1 ? '是' : '否' }}
          </el-tag>
        </template>
      </el-table-column>
    </el-table>
    <p class="dim">
      幂等键是 <code>(entity_type, entity_id, kind, occurred_at)</code>：SDK 的指纹闸只挡「同一条报文」，
      官方超时重推会以新报文再打一次，业务幂等必须落库才能跨重启生效。
    </p>
  </div>
</template>
