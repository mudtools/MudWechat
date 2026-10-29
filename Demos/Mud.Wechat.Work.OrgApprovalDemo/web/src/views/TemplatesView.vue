<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Refresh } from '@element-plus/icons-vue'

import {
  confirmRegistry,
  ignoreRegistry,
  listRegistry,
  listTemplates,
  registerTemplate,
  startDiscovery,
  startTemplateSync,
  syncOneTemplate,
} from '../api/endpoints'
import type { RegistryRow, TemplateRow } from '../api/types'
import { useHealth } from '../composables/useHealth'
import { discoverySourceText, formatUnix, templateStatusText, templateStatusType } from '../utils/format'
import { presentError, presentSuccess } from '../utils/notify'

const router = useRouter()
const { writeEnabled } = useHealth()

const rows = ref<TemplateRow[]>([])
const pending = ref<RegistryRow[]>([])
const loading = ref(false)
const hint = ref('')
const registerInput = ref('')
const busy = reactive({ register: false, batch: false, discovery: false, rowId: '' })

async function load(): Promise<void> {
  loading.value = true
  try {
    const [listResult, registryResult] = await Promise.all([listTemplates(), listRegistry('pending')])
    rows.value = listResult.items
    hint.value = listResult.hint
    pending.value = registryResult.items
  } catch (reason) {
    presentError(reason)
  } finally {
    loading.value = false
  }
}

async function register(): Promise<void> {
  const templateId = registerInput.value.trim()
  if (!templateId) {
    ElMessage.warning('请填写模板 id')
    return
  }

  busy.register = true
  try {
    await registerTemplate(templateId)
    presentSuccess(`已登记 ${templateId}，可以点该行的「同步」拉取详情`)
    registerInput.value = ''
    await load()
  } catch (reason) {
    presentError(reason)
  } finally {
    busy.register = false
  }
}

async function syncOne(templateId: string): Promise<void> {
  busy.rowId = templateId
  try {
    const result = await syncOneTemplate(templateId)
    presentSuccess(result.createdNewVersion ? '已产生新版本快照' : '内容未变，版本号未变（幂等）')
    await load()
  } catch (reason) {
    presentError(reason)
  } finally {
    busy.rowId = ''
  }
}

async function syncAll(): Promise<void> {
  busy.batch = true
  try {
    const result = await startTemplateSync()
    presentSuccess(`已投递模板同步任务 #${result.runId}（进度见同步中心）`)
  } catch (reason) {
    presentError(reason)
  } finally {
    busy.batch = false
  }
}

async function discover(): Promise<void> {
  busy.discovery = true
  try {
    const result = await startDiscovery()
    presentSuccess(`已投递回捞发现任务 #${result.runId}`)
  } catch (reason) {
    presentError(reason)
  } finally {
    busy.discovery = false
  }
}

async function confirmRow(templateId: string): Promise<void> {
  try {
    await confirmRegistry(templateId)
    presentSuccess(`已登记 ${templateId}`)
    await load()
  } catch (reason) {
    presentError(reason)
  }
}

async function ignoreRow(templateId: string): Promise<void> {
  try {
    await ignoreRegistry(templateId)
    presentSuccess(`已忽略 ${templateId}`)
    await load()
  } catch (reason) {
    presentError(reason)
  }
}

onMounted(load)
</script>

<template>
  <div class="panel">
    <div class="toolbar">
      <el-input
        v-model="registerInput"
        size="small"
        placeholder="粘贴审批模板 id（管理后台模板编辑页 URL 里那一段）"
        style="width: 360px"
        :disabled="!writeEnabled"
        @keyup.enter="register"
      />
      <el-button size="small" type="primary" :loading="busy.register" :disabled="!writeEnabled" @click="register">
        登记模板 id
      </el-button>
      <el-button size="small" :loading="busy.batch" :disabled="!writeEnabled" @click="syncAll">同步全部</el-button>
      <el-button size="small" :loading="busy.discovery" :disabled="!writeEnabled" @click="discover">
        回捞发现（默认关）
      </el-button>
      <el-button size="small" :disabled="!writeEnabled" @click="router.push('/templates/new/edit')">
        新建模板
      </el-button>
      <span class="toolbar__spacer" />
      <el-button size="small" :icon="Refresh" :loading="loading" @click="load">刷新</el-button>
    </div>

    <el-alert type="warning" :closable="false" show-icon title="官方没有「列出企业全部审批模板」的接口" style="margin-bottom: 10px">
      <template #default>
        <p style="margin: 0">
          {{ hint }}
        </p>
        <p style="margin: 4px 0 0">
          template_id 只有三个合法来源：① 管理后台模板编辑页 URL 手工复制；②
          <code>sys_approval_change</code> 回调载荷的 <code>TemplateId</code>；③
          <code>oa/getapprovaldetail</code> 应答的 <code>info.template_id</code>。
          自动发现只入「待登记」，必须人工确认后才进同步队列。
        </p>
      </template>
    </el-alert>

    <h3 class="panel__title">已登记模板（本地台账）</h3>
    <el-table :data="rows" size="small" border v-loading="loading">
      <el-table-column prop="templateId" label="模板 id" min-width="180">
        <template #default="{ row }"><span class="mono">{{ row.templateId }}</span></template>
      </el-table-column>
      <el-table-column prop="name" label="名称" width="180" />
      <el-table-column prop="controlCount" label="控件数" width="90" />
      <el-table-column prop="version" label="版本" width="80" />
      <el-table-column label="最后同步" width="160">
        <template #default="{ row }">{{ formatUnix(row.lastSyncedAt) }}</template>
      </el-table-column>
      <el-table-column label="状态" width="120">
        <template #default="{ row }">
          <el-tag size="small" :type="templateStatusType(row.status)">{{ templateStatusText(row.status) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="来源" width="120">
        <template #default="{ row }">{{ discoverySourceText(row.discoverySource) }}</template>
      </el-table-column>
      <el-table-column label="操作" width="270" fixed="right">
        <template #default="{ row }">
          <el-button
            link
            size="small"
            :loading="busy.rowId === row.templateId"
            :disabled="!writeEnabled"
            @click="syncOne(row.templateId)"
          >
            同步
          </el-button>
          <el-button link size="small" @click="router.push(`/templates/${row.templateId}`)">详情</el-button>
          <el-button link size="small" @click="router.push(`/templates/${row.templateId}/diff`)">版本</el-button>
          <el-button
            link
            size="small"
            :disabled="!writeEnabled"
            @click="router.push(`/templates/${row.templateId}/edit`)"
          >
            编辑
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <template v-if="pending.length > 0">
      <h3 class="panel__title" style="margin-top: 16px">待登记队列（{{ pending.length }}）</h3>
      <el-table :data="pending" size="small" border>
        <el-table-column prop="templateId" label="模板 id" min-width="180">
          <template #default="{ row }"><span class="mono">{{ row.templateId }}</span></template>
        </el-table-column>
        <el-table-column label="发现来源" width="140">
          <template #default="{ row }">{{ discoverySourceText(row.discoveredFrom) }}</template>
        </el-table-column>
        <el-table-column label="首次发现" width="170">
          <template #default="{ row }">{{ formatUnix(row.firstSeenAt) }}</template>
        </el-table-column>
        <el-table-column prop="note" label="备注" min-width="180" />
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="{ row }">
            <el-button link size="small" :disabled="!writeEnabled" @click="confirmRow(row.templateId)">确认登记</el-button>
            <el-button link size="small" :disabled="!writeEnabled" @click="ignoreRow(row.templateId)">忽略</el-button>
          </template>
        </el-table-column>
      </el-table>
    </template>
  </div>
</template>
