<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import { getTemplateDiff, listTemplateVersions } from '../api/endpoints'
import type { ControlDiff, TemplateDiffResult, TemplateVersionRow } from '../api/types'
import { formatUnix } from '../utils/format'
import { presentError } from '../utils/notify'

const route = useRoute()
const router = useRouter()

const templateId = String(route.params.id ?? '')
const versions = ref<TemplateVersionRow[]>([])
const fromVersion = ref<number | null>(null)
const toVersion = ref<number | null>(null)
const diff = ref<TemplateDiffResult | null>(null)
const loading = ref(false)
const error = ref('')

const fromOptions = computed(() => versions.value.filter((v) => toVersion.value === null || v.version < toVersion.value))

function fieldName(field: string): string {
  switch (field) {
    case 'control':
      return '控件类型'
    case 'title':
      return '标题'
    case 'require':
      return '必填'
    case 'unPrint':
      return '参与打印（un_print）'
    case 'orderNo':
      return '顺序'
    case 'config':
      return '控件配置（config）'
    default:
      return field
  }
}

async function loadVersions(): Promise<void> {
  if (!templateId) {
    error.value = '路由缺少模板 id'
    return
  }

  try {
    const result = await listTemplateVersions(templateId)
    versions.value = result.items
    toVersion.value = result.items[0]?.version ?? null
    fromVersion.value = result.items[1]?.version ?? null
    await loadDiff()
  } catch (reason) {
    error.value = (reason as Error).message
    presentError(reason)
  }
}

async function loadDiff(): Promise<void> {
  if (!templateId) {
    return
  }

  loading.value = true
  error.value = ''
  try {
    diff.value = await getTemplateDiff(
      templateId,
      fromVersion.value ?? undefined,
      toVersion.value ?? undefined,
    )
  } catch (reason) {
    error.value = (reason as Error).message
    presentError(reason)
  } finally {
    loading.value = false
  }
}

function changeRows(rows: ControlDiff[] | undefined): ControlDiff[] {
  return rows ?? []
}

onMounted(loadVersions)
</script>

<template>
  <div class="panel" v-loading="loading">
    <div class="toolbar">
      <el-button size="small" @click="router.push('/templates')">返回列表</el-button>
      <el-button size="small" @click="router.push(`/templates/${templateId}`)">模板详情</el-button>
      <span class="toolbar__spacer" />
      <span class="dim">从</span>
      <el-select v-model="fromVersion" size="small" style="width: 150px" clearable placeholder="（空 = 首版）">
        <el-option
          v-for="item in fromOptions"
          :key="item.version"
          :label="`v${item.version} · ${formatUnix(item.captureAt)}`"
          :value="item.version"
        />
      </el-select>
      <span class="dim">到</span>
      <el-select v-model="toVersion" size="small" style="width: 150px">
        <el-option
          v-for="item in versions"
          :key="item.version"
          :label="`v${item.version} · ${formatUnix(item.captureAt)}`"
          :value="item.version"
        />
      </el-select>
      <el-button size="small" type="primary" @click="loadDiff">比对</el-button>
    </div>

    <el-alert v-if="error" type="error" :closable="false" :title="error" style="margin-bottom: 12px" />

    <p v-if="diff" class="dim">
      比对结果：新增 {{ diff.added.length }} / 删除 {{ diff.removed.length }} / 变更 {{ diff.changed.length }}
      （按 <b>控件 id</b> 匹配，不按位置也不按标题）
    </p>

    <el-tabs v-if="diff" type="border-card">
      <el-tab-pane :label="`新增 (${diff.added.length})`" name="added">
        <el-table :data="changeRows(diff.added)" size="small" border>
          <el-table-column label="控件 id" width="220">
            <template #default="{ row }"><span class="mono">{{ row.controlId }}</span></template>
          </el-table-column>
          <el-table-column prop="title" label="标题" min-width="200" />
        </el-table>
      </el-tab-pane>

      <el-tab-pane :label="`删除 (${diff.removed.length})`" name="removed">
        <el-table :data="changeRows(diff.removed)" size="small" border>
          <el-table-column label="控件 id" width="220">
            <template #default="{ row }"><span class="mono">{{ row.controlId }}</span></template>
          </el-table-column>
          <el-table-column prop="title" label="标题" min-width="200" />
        </el-table>
        <p class="dim">删除行意味着这些控件在「到」版本里已不存在（覆盖式更新的后果之一）。</p>
      </el-tab-pane>

      <el-tab-pane :label="`变更 (${diff.changed.length})`" name="changed">
        <template v-for="row in changeRows(diff.changed)" :key="row.controlId">
          <h4 style="margin: 10px 0 6px">
            <span class="mono">{{ row.controlId }}</span>
            <span class="dim"> · {{ row.title ?? '（无标题）' }}</span>
          </h4>
          <el-table :data="row.fields" size="small" border>
            <el-table-column label="字段" width="180">
              <template #default="{ row: field }">{{ fieldName(field.field) }}</template>
            </el-table-column>
            <el-table-column label="旧值" min-width="220">
              <template #default="{ row: field }">
                <span class="mono" style="background: #fef0f0; display: block; padding: 2px 4px">{{ field.before ?? '（无）' }}</span>
              </template>
            </el-table-column>
            <el-table-column label="新值" min-width="220">
              <template #default="{ row: field }">
                <span class="mono" style="background: #f0f9eb; display: block; padding: 2px 4px">{{ field.after ?? '（无）' }}</span>
              </template>
            </el-table-column>
          </el-table>
        </template>
        <p v-if="changeRows(diff.changed).length === 0" class="dim">两个版本的控件字段完全一致。</p>
      </el-tab-pane>
    </el-tabs>

    <el-table v-if="versions.length" :data="versions" size="small" border style="margin-top: 16px">
      <el-table-column prop="version" label="版本" width="80" />
      <el-table-column label="抓取时间" width="170">
        <template #default="{ row }">{{ formatUnix(row.captureAt) }}</template>
      </el-table-column>
      <el-table-column prop="changeSummary" label="变更摘要" min-width="220" />
      <el-table-column label="内容 hash" min-width="280">
        <template #default="{ row }"><span class="mono">{{ row.contentHash }}</span></template>
      </el-table-column>
    </el-table>
  </div>
</template>
