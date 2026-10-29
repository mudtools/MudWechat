<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import { getTemplate } from '../api/endpoints'
import type { TemplateDetail } from '../api/types'
import { discoverySourceText, formatUnix, printFlagText, requireText, templateStatusText } from '../utils/format'
import { presentError } from '../utils/notify'

const route = useRoute()
const router = useRouter()

const detail = ref<TemplateDetail | null>(null)
const loading = ref(false)
const error = ref('')

const templateId = String(route.params.id ?? '')

async function load(): Promise<void> {
  if (!templateId) {
    error.value = '路由缺少模板 id'
    return
  }

  loading.value = true
  error.value = ''
  try {
    detail.value = await getTemplate(templateId)
  } catch (reason) {
    error.value = (reason as Error).message
    presentError(reason)
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="panel" v-loading="loading">
    <div class="toolbar">
      <el-button size="small" @click="router.push('/templates')">返回列表</el-button>
      <el-button size="small" @click="router.push(`/templates/${templateId}/diff`)">版本对比</el-button>
      <el-button size="small" @click="router.push(`/templates/${templateId}/edit`)">维护控件</el-button>
      <span class="toolbar__spacer" />
      <span class="mono dim">{{ templateId }}</span>
    </div>

    <el-alert v-if="error" type="error" :closable="false" :title="error" style="margin-bottom: 12px" />

    <template v-if="detail">
      <h3 class="panel__title">
        {{ detail.name ?? '（无名称）' }}
        <el-tag size="small" type="info" effect="plain">v{{ detail.version }}</el-tag>
        <el-tag size="small" :type="detail.status === 2 ? 'info' : 'success'">
          {{ templateStatusText(detail.status) }}
        </el-tag>
      </h3>

      <p class="dim">
        <span class="field-label">英文名</span>{{ detail.nameEn ?? '—' }}<br />
        <span class="field-label">控件数</span>{{ detail.controlCount }}（顶层；明细子控件见下表展开行）<br />
        <span class="field-label">最后同步</span>{{ formatUnix(detail.lastSyncedAt) }}<br />
        <span class="field-label">发现来源</span>{{ discoverySourceText(detail.discoverySource) }}
      </p>

      <el-alert type="info" :closable="false" show-icon style="margin-bottom: 10px">
        <template #default>
          流程节点（审批人 / 条件分支 / 抄送）<b>不可经 API 读取也不可写</b>：官方
          <code>update_template</code> 明确「已配置的审批流程和规则不变」，请在企微管理后台配置。
        </template>
      </el-alert>

      <el-table
        :data="detail.controls"
        size="small"
        border
        row-key="controlId"
        :tree-props="{ children: 'children' }"
        default-expand-all
      >
        <el-table-column label="类型" width="150">
          <template #default="{ row }">
            <el-tag size="small" effect="plain">{{ row.control ?? '—' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="title" label="标题" min-width="180" />
        <el-table-column label="必填" width="100">
          <template #default="{ row }">{{ requireText(row.require) }}</template>
        </el-table-column>
        <el-table-column label="参与打印" width="110">
          <!-- un_print 语义反转：1 = 不参与打印，列头写「参与打印」必须取反。 -->
          <template #default="{ row }">{{ printFlagText(row.unPrint) }}</template>
        </el-table-column>
        <el-table-column label="控件 id" width="200">
          <template #default="{ row }"><span class="mono">{{ row.controlId }}</span></template>
        </el-table-column>
      </el-table>

      <el-collapse style="margin-top: 12px">
        <el-collapse-item title="原始 JSON（规范化后的应答原文，可逐字节比对）" name="raw">
          <pre class="mono" style="max-height: 420px; overflow: auto; white-space: pre-wrap">{{ detail.rawJson }}</pre>
        </el-collapse-item>
      </el-collapse>
    </template>
  </div>
</template>
