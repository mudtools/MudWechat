<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'

import { createTemplate, dryRunTemplate, getTemplate, updateTemplate } from '../api/endpoints'
import type { ControlNode, SettingControl, TemplateDiffResult } from '../api/types'
import { useHealth } from '../composables/useHealth'
import { presentError, presentSuccess } from '../utils/notify'

/**
 * 模板侧控件的 17 个官方类型（**没有 `PunchCorrection`**：它只存在于单据侧 18 值里，
 * 差异属官方，勿「顺手对齐」）。
 */
const CONTROL_TYPES = [
  'Text',
  'Textarea',
  'Number',
  'Money',
  'Date',
  'Selector',
  'Contact',
  'Tips',
  'File',
  'Table',
  'Attendance',
  'Vacation',
  'Location',
  'RelatedApproval',
  'Formula',
  'DateRange',
  'BankAccount',
]

interface EditorControl {
  controlId: string
  control: string
  title: string
  require: number
  unPrint: number
  configText: string
  children: EditorControl[]
}

const route = useRoute()
const router = useRouter()
const { writeEnabled } = useHealth()

const rawId = String(route.params.id ?? '')
const isCreate = rawId === 'new'
const templateId = isCreate ? '' : rawId

const name = ref('')
const version = ref(0)
const controls = ref<EditorControl[]>([])
const loading = ref(false)
const submitting = ref(false)
const error = ref('')

const dryRunResult = ref<(TemplateDiffResult & { hint: string }) | null>(null)
const confirmDialog = reactive({ visible: false, acknowledged: false })
const selectedId = ref('')

function toEditorControl(node: ControlNode): EditorControl {
  return {
    controlId: node.controlId,
    control: node.control ?? 'Text',
    title: node.title ?? '',
    require: node.require ?? 0,
    unPrint: node.unPrint ?? 0,
    configText: node.config ? JSON.stringify(node.config, null, 2) : '',
    children: (node.children ?? []).map(toEditorControl),
  }
}

/** 遍历整棵树（含明细子控件）。 */
function walk(nodes: EditorControl[], visit: (node: EditorControl, parent: EditorControl | null) => void): void {
  for (const node of nodes) {
    visit(node, null)
    for (const child of node.children) {
      visit(child, node)
      walk(child.children, visit)
    }
  }
}

function findById(id: string): EditorControl | null {
  let found: EditorControl | null = null
  walk(controls.value, (node) => {
    if (node.controlId === id) {
      found = node
    }
  })
  return found
}

const selected = computed<EditorControl | null>(() => (selectedId.value ? findById(selectedId.value) : null))

async function load(): Promise<void> {
  if (isCreate) {
    name.value = ''
    version.value = 0
    controls.value = []
    return
  }

  loading.value = true
  error.value = ''
  try {
    const detail = await getTemplate(templateId)
    name.value = detail.name ?? ''
    version.value = detail.version
    controls.value = detail.controls.map(toEditorControl)
  } catch (reason) {
    error.value = (reason as Error).message
    presentError(reason)
  } finally {
    loading.value = false
  }
}

function parseConfig(text: string, controlId: string): Record<string, unknown> | null {
  if (!text.trim()) {
    return null
  }

  try {
    const parsed = JSON.parse(text) as unknown
    if (parsed === null || typeof parsed !== 'object' || Array.isArray(parsed)) {
      throw new Error('config 必须是 JSON 对象')
    }

    return parsed as Record<string, unknown>
  } catch (reason) {
    throw new Error(`控件 ${controlId} 的 config 不是合法 JSON 对象：${(reason as Error).message}`)
  }
}

function toSettingControl(node: EditorControl): SettingControl {
  return {
    property: {
      control: node.control,
      id: node.controlId,
      title: [{ text: node.title, lang: 'zh_CN' }],
      require: node.require,
      // un_print 语义反转：1 = 不参与打印、0 = 参与打印。
      un_print: node.unPrint,
    },
    config: parseConfig(node.configText, node.controlId),
  }
}

function buildTree(nodes: EditorControl[]): SettingControl[] {
  return nodes.map((node) => {
    const setting = toSettingControl(node)
    if (node.children.length > 0) {
      // 明细控件的 children 与 controls 同形（config.table.children）。
      const existingTable = (setting.config?.table as Record<string, unknown> | undefined) ?? {}
      setting.config = {
        ...(setting.config ?? {}),
        table: { ...existingTable, children: buildTree(node.children) },
      }
    }

    return setting
  })
}

function nextControlId(): string {
  return `ctrl_${Date.now().toString(36)}_${Math.floor(Math.random() * 1000)}`
}

function addRootControl(): void {
  const node: EditorControl = {
    controlId: nextControlId(),
    control: 'Text',
    title: `新控件${controls.value.length + 1}`,
    require: 0,
    unPrint: 1,
    configText: '',
    children: [],
  }
  controls.value = [...controls.value, node]
  selectedId.value = node.controlId
}

function addChildControl(parent: EditorControl): void {
  if (parent.control !== 'Table') {
    ElMessage.warning('只有明细（Table）控件可以拥有子控件')
    return
  }

  const node: EditorControl = {
    controlId: nextControlId(),
    control: 'Text',
    title: `新子控件${parent.children.length + 1}`,
    require: 0,
    unPrint: 1,
    configText: '',
    children: [],
  }
  parent.children = [...parent.children, node]
  selectedId.value = node.controlId
}

function detach(node: EditorControl): { list: EditorControl[]; index: number } | null {
  let result: { list: EditorControl[]; index: number } | null = null
  const scan = (list: EditorControl[]): void => {
    const index = list.indexOf(node)
    if (index >= 0) {
      result = { list, index }
      return
    }
    for (const item of list) {
      scan(item.children)
    }
  }
  scan(controls.value)
  return result
}

function removeControl(node: EditorControl): void {
  const location = detach(node)
  if (!location) {
    return
  }

  location.list.splice(location.index, 1)
  if (selectedId.value === node.controlId) {
    selectedId.value = ''
  }
}

function move(node: EditorControl, delta: number): void {
  const location = detach(node)
  if (!location) {
    return
  }

  const target = location.index + delta
  if (target < 0 || target >= location.list.length) {
    return
  }

  location.list.splice(location.index, 1)
  location.list.splice(target, 0, node)
}

function select(node: EditorControl): void {
  selectedId.value = node.controlId
}

function setRequire(value: string | number | boolean): void {
  const node = selected.value
  if (node) {
    node.require = value === true || value === 'true' ? 1 : 0
  }
}

function setPrintEnabled(value: string | number | boolean): void {
  const node = selected.value
  if (node) {
    // 界面写「参与打印」，官方字段是 un_print ⇒ 取反。
    node.unPrint = value === true || value === 'true' ? 0 : 1
  }
}

const totalCount = computed(() => {
  let count = 0
  walk(controls.value, () => {
    count += 1
  })
  return count
})

function preflightIssue(): string | null {
  if (!name.value.trim()) {
    return '模板名不能为空'
  }

  if (name.value.length > 40) {
    return '模板名不得超过 40 个字符（官方命名闸）'
  }

  if (controls.value.length === 0) {
    return '至少需要一个控件'
  }

  const titles = new Set<string>()
  const fakeKinds = new Set<string>()
  let issue: string | null = null

  walk(controls.value, (node) => {
    if (issue) {
      return
    }
    if (!node.controlId.trim()) {
      issue = '存在控件 id 为空的控件'
      return
    }
    if (node.title.length > 40) {
      issue = `控件 ${node.controlId} 的标题超过 40 个字符`
      return
    }
    if (titles.has(node.title)) {
      issue = `控件标题重复：${node.title}（官方要求控件名不得重名）`
      return
    }
    titles.add(node.title)

    if (node.control === 'Vacation' || node.control === 'Attendance') {
      fakeKinds.add(node.control)
    }
  })

  if (issue) {
    return issue
  }

  // 第四道命名闸：一个模板只能拥有一类假勤控件（Vacation 与 Attendance 二选一）。
  if (fakeKinds.size > 1) {
    return '一个模板只能拥有一类假勤控件（Vacation 与 Attendance 二选一）'
  }

  return null
}

async function startSubmit(): Promise<void> {
  const issue = preflightIssue()
  if (issue) {
    ElMessage.warning(issue)
    return
  }

  let settingControls: SettingControl[]
  try {
    settingControls = buildTree(controls.value)
  } catch (reason) {
    ElMessage.error((reason as Error).message)
    return
  }

  submitting.value = true
  try {
    // 强制先 dry-run：覆盖式替换的后果必须让使用者看见（方案 §4.3 / §10）。
    const result = await dryRunTemplate(isCreate ? null : templateId, settingControls)
    dryRunResult.value = result
    confirmDialog.acknowledged = false
    confirmDialog.visible = true
  } catch (reason) {
    presentError(reason)
  } finally {
    submitting.value = false
  }
}

async function doSubmit(): Promise<void> {
  if (!confirmDialog.acknowledged) {
    ElMessage.warning('请先勾选确认项')
    return
  }

  let settingControls: SettingControl[]
  try {
    settingControls = buildTree(controls.value)
  } catch (reason) {
    ElMessage.error((reason as Error).message)
    return
  }

  submitting.value = true
  try {
    if (isCreate) {
      const result = await createTemplate(name.value.trim(), settingControls)
      presentSuccess(`模板已创建：${result.templateId}`)
      confirmDialog.visible = false
      await router.push(`/templates/${result.templateId}`)
      return
    }

    const result = await updateTemplate(templateId, version.value, name.value.trim(), settingControls)
    presentSuccess(`模板已更新，当前版本 v${result.version}`)
    confirmDialog.visible = false
    await load()
  } catch (reason) {
    presentError(reason)
  } finally {
    submitting.value = false
  }
}

function handleRowClick(row: EditorControl): void {
  select(row)
}

onMounted(load)
</script>

<template>
  <div class="panel" v-loading="loading">
    <el-alert
      type="warning"
      :closable="false"
      show-icon
      class="banner-warning"
      title="流程节点（审批人 / 条件分支 / 抄送）只能在企业微信管理后台配置，本页仅维护表单控件"
    >
      <template #default>
        官方 <code>update_template</code> 原文：「更新模板后管理后台及审批应用内将更新原模板的内容，
        <b>已配置的审批流程和规则不变</b>」。本页<b>不是</b>流程编辑器。
      </template>
    </el-alert>

    <el-alert type="error" :closable="false" show-icon class="banner-warning" title="update_template 是覆盖式全量替换">
      <template #default>
        提交的 <code>controls</code> 就是模板的全部控件 —— <b>漏传即等于删除</b>。提交前会强制 dry-run
        展示差异，并带 <code>baseVersion</code> 乐观锁防止覆盖他人改动。
      </template>
    </el-alert>

    <div class="toolbar">
      <el-button size="small" @click="router.push('/templates')">返回列表</el-button>
      <span class="dim">{{ isCreate ? '新建模板' : `模板 ${templateId} · 当前 v${version}` }}</span>
      <span class="toolbar__spacer" />
      <el-button size="small" :disabled="!writeEnabled" @click="addRootControl">添加控件</el-button>
      <el-button size="small" type="primary" :loading="submitting" :disabled="!writeEnabled" @click="startSubmit">
        校验并提交（先 dry-run）
      </el-button>
    </div>

    <el-alert v-if="error" type="error" :closable="false" :title="error" style="margin-bottom: 12px" />

    <el-form label-width="90px" style="max-width: 520px">
      <el-form-item label="模板名">
        <el-input v-model="name" maxlength="40" show-word-limit placeholder="≤ 40 字符且不得重名" />
      </el-form-item>
    </el-form>

    <div class="split" style="margin-top: 8px">
      <div class="split__main panel" style="border: none; padding: 0">
        <el-table
          :data="controls"
          size="small"
          border
          row-key="controlId"
          :tree-props="{ children: 'children' }"
          default-expand-all
          highlight-current-row
          @row-click="handleRowClick"
        >
          <el-table-column label="类型" width="140">
            <template #default="{ row }">
              <el-tag size="small" effect="plain">{{ row.control }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="title" label="标题" min-width="160" />
          <el-table-column label="必填" width="90">
            <template #default="{ row }">{{ row.require === 1 ? '必填' : '非必填' }}</template>
          </el-table-column>
          <el-table-column label="参与打印" width="100">
            <template #default="{ row }">{{ row.unPrint === 1 ? '不参与' : '参与' }}</template>
          </el-table-column>
          <el-table-column label="控件 id" width="200">
            <template #default="{ row }"><span class="mono">{{ row.controlId }}</span></template>
          </el-table-column>
          <el-table-column label="操作" width="250" fixed="right">
            <template #default="{ row }">
              <el-button link size="small" @click.stop="select(row)">编辑</el-button>
              <el-button link size="small" @click.stop="move(row, -1)">上移</el-button>
              <el-button link size="small" @click.stop="move(row, 1)">下移</el-button>
              <el-button v-if="row.control === 'Table'" link size="small" @click.stop="addChildControl(row)">
                加子控件
              </el-button>
              <el-button link size="small" type="danger" @click.stop="removeControl(row)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
        <p class="dim">共 {{ totalCount }} 个控件（含明细子控件）。</p>
      </div>

      <div class="split__aside panel">
        <h3 class="panel__title">属性面板</h3>
        <template v-if="selected">
          <el-form label-width="80px" size="small">
            <el-form-item label="控件 id">
              <el-input v-model="selected.controlId" />
            </el-form-item>
            <el-form-item label="类型">
              <el-select v-model="selected.control" style="width: 100%">
                <el-option v-for="item in CONTROL_TYPES" :key="item" :label="item" :value="item" />
              </el-select>
            </el-form-item>
            <el-form-item label="标题">
              <el-input v-model="selected.title" maxlength="40" show-word-limit />
            </el-form-item>
            <el-form-item label="必填">
              <el-switch :model-value="selected.require === 1" @update:model-value="setRequire" />
            </el-form-item>
            <el-form-item label="参与打印">
              <el-switch :model-value="selected.unPrint === 0" @update:model-value="setPrintEnabled" />
            </el-form-item>
            <el-form-item label="config">
              <el-input
                v-model="selected.configText"
                type="textarea"
                :rows="8"
                placeholder="空 = 不提交 config（Text/Textarea/Number/Money 等无需配置）"
              />
            </el-form-item>
          </el-form>
          <p class="dim">
            响应侧 <code>config</code> 与请求侧 <code>config</code> 是<b>两个类型</b>
            （<code>ApprovalTemplateControlConfig</code> vs <code>ApprovalTemplateSettingControlConfig</code>），
            原样回写仅对结构兼容的控件成立；不兼容时请按官方文档改写本段 JSON。
          </p>
        </template>
        <p v-else class="dim">在左侧选择一行控件以编辑属性。</p>
      </div>
    </div>

    <!-- dry-run 结果 + 二次确认 -->
    <el-dialog v-model="confirmDialog.visible" title="提交前确认（覆盖式替换）" width="720px">
      <template v-if="dryRunResult">
        <p>
          本次提交相对
          <template v-if="isCreate">「空模板」</template>
          <template v-else>「当前版本 v{{ version }}」</template>
          的差异：<b>新增 {{ dryRunResult.added.length }}</b> /
          <b>删除 {{ dryRunResult.removed.length }}</b> / <b>变更 {{ dryRunResult.changed.length }}</b>
        </p>

        <el-table v-if="dryRunResult.removed.length" :data="dryRunResult.removed" size="small" border>
          <el-table-column label="将被删除的控件" min-width="200">
            <template #default="{ row }">
              <span class="mono">{{ row.controlId }}</span> · {{ row.title ?? '（无标题）' }}
            </template>
          </el-table-column>
        </el-table>

        <ul style="max-height: 200px; overflow: auto">
          <li v-for="row in dryRunResult.changed" :key="row.controlId">
            <span class="mono">{{ row.controlId }}</span>：
            <span v-for="field in row.fields" :key="field.field" style="margin-right: 10px">
              {{ field.field }} <span class="dim">{{ field.before ?? '∅' }} → {{ field.after ?? '∅' }}</span>
            </span>
          </li>
          <li v-for="row in dryRunResult.added" :key="`add-${row.controlId}`">
            新增 <span class="mono">{{ row.controlId }}</span> · {{ row.title ?? '' }}
          </li>
        </ul>

        <el-alert type="info" :closable="false" :title="dryRunResult.hint" style="margin-bottom: 8px" />

        <el-checkbox v-model="confirmDialog.acknowledged">
          我已知流程节点（审批人 / 条件分支 / 抄送）不会随之改变，且本次提交是覆盖式全量替换
        </el-checkbox>
      </template>
      <template #footer>
        <el-button @click="confirmDialog.visible = false">取消</el-button>
        <el-button type="primary" :loading="submitting" :disabled="!confirmDialog.acknowledged" @click="doSubmit">
          确认提交
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>
