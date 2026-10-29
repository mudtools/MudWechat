<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { Search } from '@element-plus/icons-vue'

import { listJobs } from '../api/articles'
import type { JobView } from '../api/types'
import { formatIso, publishStatusTagType, publishStatusText } from '../utils/format'
import { presentError } from '../utils/notify'
import { ElMessage } from 'element-plus'

const emit = defineEmits<{ inspect: [publishId: string] }>()

const jobs = ref<JobView[]>([])
const loading = ref(false)
const hint = ref('')

async function load() {
  loading.value = true
  try {
    const result = await listJobs(50)
    jobs.value = result.items
    hint.value = result.hint ?? ''
  } catch (reason) {
    jobs.value = []
    ElMessage.error('台账读取失败：' + String((reason as Error).message))
    presentError(reason)
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div>
    <div class="panel__toolbar">
      <el-button :icon="Search" size="small" :loading="loading" @click="load">刷新台账</el-button>
      <span style="flex: 1" />
      <span class="ledger-badge">本地记录，微信为准</span>
    </div>

    <el-table v-loading="loading" :data="jobs" size="small">
      <el-table-column type="expand">
        <template #default="{ row }">
          <div style="padding: 8px 16px">
            <el-text size="small">
              媒体 id：<code>{{ row.mediaId || '（仅被单独查过状态）' }}</code>
            </el-text>
            <el-timeline v-if="row.snapshots.length" class="status-timeline">
              <el-timeline-item
                v-for="snapshot in row.snapshots"
                :key="snapshot.attemptNo"
                :timestamp="formatIso(snapshot.polledAt)"
                placement="top"
              >
                <el-tag size="small" :type="publishStatusTagType(snapshot.status)">
                  第 {{ snapshot.attemptNo }} 轮 ·
                  {{ snapshot.status === null ? '无状态' : `status=${snapshot.status}` }}
                  {{ publishStatusText(snapshot.status) }}
                </el-tag>
                <el-text v-if="snapshot.errorCode" size="small" type="danger">
                  errcode {{ snapshot.errorCode }}
                </el-text>
              </el-timeline-item>
            </el-timeline>
            <el-empty v-else description="还没有轮询记录" :image-size="60" />
          </div>
        </template>
      </el-table-column>

      <el-table-column label="publish_id" width="110">
        <template #default="{ row }">
          <el-text size="small">{{ row.publishId.slice(-10) }}</el-text>
        </template>
      </el-table-column>
      <el-table-column label="最新状态" min-width="140">
        <template #default="{ row }">
          <el-tag size="small" :type="publishStatusTagType(row.latestStatus)">
            {{ row.latestStatus === null ? '—' : `${row.latestStatus} ${publishStatusText(row.latestStatus)}` }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="轮询" width="60">
        <template #default="{ row }">{{ row.attempts }}</template>
      </el-table-column>
      <el-table-column label="提交时间" min-width="120">
        <template #default="{ row }">
          <el-text size="small">{{ formatIso(row.submittedAt) }}</el-text>
        </template>
      </el-table-column>
      <el-table-column label="" width="80">
        <template #default="{ row }">
          <el-button size="small" text type="primary" @click="emit('inspect', row.publishId)">查状态</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-text size="small" type="info">{{ hint }}</el-text>
  </div>
</template>
