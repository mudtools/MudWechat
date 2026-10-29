<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Refresh, Search } from '@element-plus/icons-vue'

import {
  createDepartment,
  createMember,
  deleteDepartment,
  deleteMember,
  getDepartment,
  getMember,
  listDepartments,
  listMembers,
  startSync,
  updateDepartment,
  updateMember,
} from '../api/endpoints'
import type { DeptDetail, DeptNode, MemberDetail, MemberRow } from '../api/types'
import { useHealth } from '../composables/useHealth'
import { deptStatusText, deptStatusType, formatUnix, memberStatusText } from '../utils/format'
import { presentError, presentSuccess } from '../utils/notify'

interface DeptTreeNode extends DeptNode {
  children: DeptTreeNode[]
}

const route = useRoute()
const router = useRouter()
const { writeEnabled } = useHealth()

const departments = ref<DeptNode[]>([])
const selectedDeptId = ref<number | null>(null)
const keyword = ref('')
const members = ref<MemberRow[]>([])
const total = ref(0)
const page = ref(1)
const size = ref(20)
const loadingMembers = ref(false)
const loadingTree = ref(false)
const includeDeleted = ref(false)

const deptNameById = computed(() => {
  const map = new Map<number, string>()
  for (const dept of departments.value) {
    map.set(dept.id, dept.name ?? `#${dept.id}`)
  }
  return map
})

const treeData = computed<DeptTreeNode[]>(() => {
  const nodes = new Map<number, DeptTreeNode>()
  for (const dept of departments.value) {
    nodes.set(dept.id, { ...dept, children: [] })
  }

  const roots: DeptTreeNode[] = []
  for (const node of nodes.values()) {
    const parent = nodes.get(node.parentId)
    if (parent && parent.id !== node.id) {
      parent.children.push(node)
    } else {
      roots.push(node)
    }
  }

  return roots
})

/** el-tree-select 需要 value/label/children 三键，与业务字段分开命名（不污染 DeptNode）。 */
interface SelectTreeNode {
  value: number
  label: string
  children: SelectTreeNode[]
}

function toSelectTree(nodes: DeptTreeNode[]): SelectTreeNode[] {
  return nodes.map((node) => ({
    value: node.id,
    label: `${node.name ?? `#${node.id}`}（${node.memberCount} 人）`,
    children: toSelectTree(node.children),
  }))
}

const selectTree = computed(() => toSelectTree(treeData.value))

const flatDeptOptions = computed(() =>
  departments.value.map((dept) => ({
    value: dept.id,
    label: `${dept.name ?? `#${dept.id}`}${dept.parentId ? '' : '（根）'}`,
  })),
)

async function loadTree(): Promise<void> {
  loadingTree.value = true
  try {
    const result = await listDepartments(includeDeleted.value)
    departments.value = result.items
  } catch (reason) {
    presentError(reason)
  } finally {
    loadingTree.value = false
  }
}

async function loadMembers(): Promise<void> {
  loadingMembers.value = true
  try {
    const result = await listMembers({
      deptId: selectedDeptId.value ?? undefined,
      keyword: keyword.value || undefined,
      page: page.value,
      size: size.value,
      includeDeleted: includeDeleted.value,
    })
    members.value = result.items
    total.value = result.total
  } catch (reason) {
    presentError(reason)
  } finally {
    loadingMembers.value = false
  }
}

function selectDept(node: DeptTreeNode | null): void {
  selectedDeptId.value = node?.id ?? null
  page.value = 1
  void loadMembers()
}

function clearDeptFilter(): void {
  selectedDeptId.value = null
  page.value = 1
  void loadMembers()
}

function onPageChange(value: number): void {
  page.value = value
  void loadMembers()
}

function onSizeChange(value: number): void {
  size.value = value
  page.value = 1
  void loadMembers()
}

async function runSync(mode: 'full' | 'reconcile'): Promise<void> {
  try {
    const result = await startSync(mode)
    presentSuccess(`已投递${mode === 'full' ? '全量同步' : '对账'}任务 #${result.runId}，正在跳转到同步中心`)
    await router.push('/sync')
  } catch (reason) {
    presentError(reason)
  }
}

// ───────────────────────── 部门抽屉 ─────────────────────────

const deptDrawer = reactive({ visible: false, id: 0, loading: false })
const deptDetail = ref<DeptDetail | null>(null)
const deptForm = reactive({
  name: '',
  nameEn: '',
  parentId: 1,
  order: 0,
  leaders: [] as string[],
})

async function openDept(id: number): Promise<void> {
  deptDrawer.id = id
  deptDrawer.visible = true
  deptDrawer.loading = true
  deptDetail.value = null
  try {
    const detail = await getDepartment(id)
    deptDetail.value = detail
    deptForm.name = detail.name ?? ''
    deptForm.nameEn = detail.nameEn ?? ''
    deptForm.parentId = detail.parentId
    deptForm.order = detail.orderNo
    deptForm.leaders = [...detail.leaders]
  } catch (reason) {
    presentError(reason)
    deptDrawer.visible = false
  } finally {
    deptDrawer.loading = false
  }
}

async function saveDept(): Promise<void> {
  if (!deptForm.name.trim()) {
    ElMessage.warning('部门名不能为空')
    return
  }

  try {
    await updateDepartment(deptDrawer.id, {
      name: deptForm.name.trim(),
      nameEn: deptForm.nameEn || null,
      parentId: deptForm.parentId,
      order: deptForm.order,
    })
    presentSuccess('部门已更新（本地镜像已同步回写；部门负责人的调整需在企微后台完成）')
    deptDrawer.visible = false
    await loadTree()
    await loadMembers()
  } catch (reason) {
    presentError(reason)
  }
}

async function removeDept(): Promise<void> {
  const id = deptDrawer.id
  try {
    // 双重保护：弹窗确认 + 后端 confirm 参数（不可逆动作，方案 §4.3）。
    await ElMessageBox.confirm(
      `删除部门不可逆（含子部门或有成员时官方会拒绝）。目标 id：${id}`,
      '确认删除部门',
      { type: 'warning', confirmButtonText: '我确定要删除', cancelButtonText: '取消' },
    )
  } catch {
    return
  }

  try {
    await deleteDepartment(id)
    presentSuccess('部门已删除')
    deptDrawer.visible = false
    await loadTree()
    await loadMembers()
  } catch (reason) {
    presentError(reason)
  }
}

// ───────────────────────── 成员抽屉 ─────────────────────────

const memberDrawer = reactive({ visible: false, userId: '', loading: false })
const memberDetail = ref<MemberDetail | null>(null)
const memberForm = reactive({
  name: '',
  position: '',
  gender: '',
  mobile: '',
  email: '',
  deptIds: [] as number[],
  mainDeptId: null as number | null,
  leaderDeptIds: [] as number[],
})

async function openMember(userId: string): Promise<void> {
  memberDrawer.userId = userId
  memberDrawer.visible = true
  memberDrawer.loading = true
  memberDetail.value = null
  try {
    const detail = await getMember(userId)
    memberDetail.value = detail
    memberForm.name = detail.name ?? ''
    memberForm.position = detail.position ?? ''
    memberForm.gender = detail.gender ?? ''
    memberForm.mobile = detail.mobile ?? ''
    memberForm.email = detail.email ?? ''
    memberForm.deptIds = [...detail.deptIds]
    memberForm.mainDeptId = detail.mainDeptId
    memberForm.leaderDeptIds = detail.deptIds.filter(
      (_deptId, index) => detail.leaderFlags[index] === 1,
    )
  } catch (reason) {
    presentError(reason)
    memberDrawer.visible = false
  } finally {
    memberDrawer.loading = false
  }
}

/**
 * 部门内负责人开关。
 *
 * 逻辑放在方法里、并返回一个「已绑好 deptId」的处理器 —— 模板表达式里不写 TS 类型标注，
 * 也不依赖模板编译器的 TS 支持（Vite 与 vue-tsc 对模板中标注的容忍度不一致）。
 */
function leaderUpdate(deptId: number): (value: string | number | boolean) => void {
  return (value: string | number | boolean): void => {
    const on = value === true || value === 'true'
    memberForm.leaderDeptIds = on
      ? Array.from(new Set([...memberForm.leaderDeptIds, deptId]))
      : memberForm.leaderDeptIds.filter((id) => id !== deptId)
  }
}

async function saveMember(): Promise<void> {
  if (memberForm.deptIds.length === 0) {
    ElMessage.warning('成员必须至少属于一个部门')
    return
  }

  try {
    await updateMember(memberDrawer.userId, {
      name: memberForm.name.trim() || null,
      position: memberForm.position || null,
      gender: memberForm.gender || null,
      mobile: memberForm.mobile || null,
      email: memberForm.email || null,
      departmentIds: memberForm.deptIds,
      mainDeptId: memberForm.mainDeptId,
      isLeaderInDept: memberForm.deptIds.map((deptId) => (memberForm.leaderDeptIds.includes(deptId) ? 1 : 0)),
    })
    presentSuccess('成员已更新')
    memberDrawer.visible = false
    await loadMembers()
  } catch (reason) {
    presentError(reason)
  }
}

async function removeMember(): Promise<void> {
  const userId = memberDrawer.userId
  try {
    await ElMessageBox.confirm(
      `删除成员不可逆：${userId}。若绑定了腾讯企业邮，邮箱账号也会一并删除。`,
      '确认删除成员',
      { type: 'warning', confirmButtonText: '我确定要删除', cancelButtonText: '取消' },
    )
  } catch {
    return
  }

  try {
    await deleteMember(userId)
    presentSuccess('成员已删除')
    memberDrawer.visible = false
    await loadMembers()
  } catch (reason) {
    presentError(reason)
  }
}

// ───────────────────────── 新建对话框 ─────────────────────────

const newDeptDialog = reactive({ visible: false, saving: false })
const newDeptForm = reactive({ name: '', nameEn: '', parentId: 1, order: 0 })

const newMemberDialog = reactive({ visible: false, saving: false })
const newMemberForm = reactive({
  userId: '',
  name: '',
  mobile: '',
  email: '',
  position: '',
  gender: '',
  deptIds: [] as number[],
  mainDeptId: null as number | null,
})

function openNewDept(): void {
  newDeptForm.name = ''
  newDeptForm.nameEn = ''
  newDeptForm.parentId = selectedDeptId.value ?? 1
  newDeptForm.order = 0
  newDeptDialog.visible = true
}

async function submitNewDept(): Promise<void> {
  if (!newDeptForm.name.trim()) {
    ElMessage.warning('部门名不能为空')
    return
  }

  newDeptDialog.saving = true
  try {
    const created = await createDepartment({
      name: newDeptForm.name.trim(),
      nameEn: newDeptForm.nameEn || null,
      parentId: newDeptForm.parentId,
      order: newDeptForm.order,
    })
    presentSuccess(`部门已创建：${created.name}（id=${created.id}）`)
    newDeptDialog.visible = false
    await loadTree()
  } catch (reason) {
    presentError(reason)
  } finally {
    newDeptDialog.saving = false
  }
}

function openNewMember(): void {
  newMemberForm.userId = ''
  newMemberForm.name = ''
  newMemberForm.mobile = ''
  newMemberForm.email = ''
  newMemberForm.position = ''
  newMemberForm.gender = ''
  newMemberForm.deptIds = selectedDeptId.value === null ? [] : [selectedDeptId.value]
  newMemberForm.mainDeptId = selectedDeptId.value
  newMemberDialog.visible = true
}

async function submitNewMember(): Promise<void> {
  if (!newMemberForm.userId.trim() || !newMemberForm.name.trim()) {
    ElMessage.warning('userid 与 name 必填')
    return
  }

  if (!newMemberForm.mobile && !newMemberForm.email) {
    ElMessage.warning('手机与邮箱不能同时为空（官方口径）')
    return
  }

  if (newMemberForm.deptIds.length === 0) {
    ElMessage.warning('请至少选择一个部门：官方对不存在的部门会自动新建，故本 Demo 只允许选本地已有部门')
    return
  }

  newMemberDialog.saving = true
  try {
    const result = await createMember({
      userId: newMemberForm.userId.trim(),
      name: newMemberForm.name.trim(),
      mobile: newMemberForm.mobile || null,
      email: newMemberForm.email || null,
      position: newMemberForm.position || null,
      gender: newMemberForm.gender || null,
      departmentIds: newMemberForm.deptIds,
      mainDeptId: newMemberForm.mainDeptId,
    })
    presentSuccess(`成员已创建：${result.userId}`)
    newMemberDialog.visible = false
    await loadMembers()
  } catch (reason) {
    presentError(reason)
  } finally {
    newMemberDialog.saving = false
  }
}

// ───────────────────────── 路由驱动抽屉 ─────────────────────────

watch(
  () => route.fullPath,
  () => {
    const deptId = route.params.id
    const userId = route.params.userId

    if (typeof deptId === 'string' && deptId.length > 0) {
      void openDept(Number(deptId))
      return
    }

    deptDrawer.visible = false

    if (typeof userId === 'string' && userId.length > 0) {
      void openMember(userId)
      return
    }

    memberDrawer.visible = false
  },
  { immediate: false },
)

onMounted(async () => {
  await loadTree()
  await loadMembers()

  const deptId = route.params.id
  const userId = route.params.userId
  if (typeof deptId === 'string' && deptId.length > 0) {
    await openDept(Number(deptId))
  } else if (typeof userId === 'string' && userId.length > 0) {
    await openMember(userId)
  }
})
</script>

<template>
  <div class="split">
    <div class="split__aside panel">
      <div class="toolbar">
        <el-button size="small" :icon="Refresh" :loading="loadingTree" @click="loadTree">刷新部门</el-button>
        <el-button size="small" :disabled="!writeEnabled" @click="openNewDept">新增部门</el-button>
      </div>

      <el-tree
        :data="treeData"
        node-key="id"
        :props="{ label: 'name', children: 'children' }"
        default-expand-all
        :expand-on-click-node="false"
        :draggable="false"
        highlight-current
        @node-click="selectDept"
      >
        <template #default="{ data }">
          <span>
            {{ data.name ?? `#${data.id}` }}
            <el-tag size="small" type="info" effect="plain">{{ data.memberCount }} 人</el-tag>
            <el-tag v-if="data.status !== 1" size="small" :type="deptStatusType(data.status)" effect="plain">
              {{ deptStatusText(data.status) }}
            </el-tag>
          </span>
        </template>
      </el-tree>

      <p class="dim" style="margin-top: 10px">
        成员数按「直属」统计；部门拖拽已禁用 —— 调整层级请在企微后台或用「编辑父部门」提交。
      </p>
    </div>

    <div class="split__main panel">
      <div class="toolbar">
        <el-input
          v-model="keyword"
          size="small"
          placeholder="搜索姓名 / 账号 / 职务"
          style="width: 220px"
          clearable
          @keyup.enter="page = 1; loadMembers()"
        />
        <el-button size="small" :icon="Search" @click="page = 1; loadMembers()">搜索</el-button>
        <el-button v-if="selectedDeptId !== null" size="small" @click="clearDeptFilter">清除部门过滤</el-button>
        <el-tag size="small" type="info" effect="plain">
          当前部门：{{ selectedDeptId === null ? '全部' : deptNameById.get(selectedDeptId) }}
        </el-tag>
        <el-checkbox v-model="includeDeleted" size="small" @change="loadTree(); loadMembers()">含已删除</el-checkbox>

        <span class="toolbar__spacer" />

        <el-button size="small" :disabled="!writeEnabled" @click="runSync('full')">全量同步</el-button>
        <el-button size="small" :disabled="!writeEnabled" @click="runSync('reconcile')">对账</el-button>
        <el-button size="small" type="primary" :disabled="!writeEnabled" @click="openNewMember">新增成员</el-button>
      </div>

      <el-table :data="members" size="small" border height="calc(100vh - 300px)" v-loading="loadingMembers">
        <el-table-column prop="userId" label="账号" width="140" />
        <el-table-column prop="name" label="姓名" width="110" />
        <el-table-column prop="position" label="职务" width="120" />
        <el-table-column label="主部门" width="140">
          <template #default="{ row }">
            {{ row.mainDeptId === null ? '—' : deptNameById.get(row.mainDeptId) ?? `#${row.mainDeptId}` }}
          </template>
        </el-table-column>
        <el-table-column label="所属部门" min-width="200">
          <template #default="{ row }">
            <el-tag
              v-for="deptId in row.deptIds"
              :key="deptId"
              size="small"
              effect="plain"
              style="margin-right: 4px"
            >
              {{ deptNameById.get(deptId) ?? `#${deptId}` }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="负责人" width="80">
          <template #default="{ row }">
            <el-tag v-if="row.isLeader" size="small" type="success">是</el-tag>
            <span v-else class="dim">—</span>
          </template>
        </el-table-column>
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            {{ memberStatusText(row.status) }}
            <el-tag v-if="row.localStatus !== 1" size="small" type="warning" effect="plain">
              {{ row.localStatus === 3 ? '已删除' : '疑似删除' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="mobileMasked" label="手机（掩码）" width="130" />
        <el-table-column prop="emailMasked" label="邮箱（掩码）" width="160" />
        <el-table-column label="快照时间" width="160">
          <template #default="{ row }">{{ formatUnix(row.syncedAt) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="90" fixed="right">
          <template #default="{ row }">
            <el-button link size="small" @click="router.push(`/org/member/${row.userId}`)">详情</el-button>
          </template>
        </el-table-column>
      </el-table>

      <el-pagination
        :current-page="page"
        :page-size="size"
        :total="total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next"
        size="small"
        style="margin-top: 8px"
        @current-change="onPageChange"
        @size-change="onSizeChange"
      />
    </div>

    <!-- 部门抽屉 -->
    <el-drawer v-model="deptDrawer.visible" title="部门详情" size="460px" @closed="router.push('/org')">
      <div v-loading="deptDrawer.loading">
        <el-form v-if="deptDetail" label-width="96px">
          <el-form-item label="部门 id">
            <span class="mono">{{ deptDetail.id }}</span>
          </el-form-item>
          <el-form-item label="名称">
            <el-input v-model="deptForm.name" :disabled="!writeEnabled" />
          </el-form-item>
          <el-form-item label="英文名">
            <el-input v-model="deptForm.nameEn" :disabled="!writeEnabled" />
          </el-form-item>
          <el-form-item label="父部门">
            <el-tree-select
              v-model="deptForm.parentId"
              :data="selectTree"
              check-strictly
              :render-after-expand="false"
              style="width: 100%"
              :disabled="!writeEnabled || deptDetail.id === 1"
            />
          </el-form-item>
          <el-form-item label="排序值">
            <el-input-number v-model="deptForm.order" :disabled="!writeEnabled" />
            <span class="dim" style="margin-left: 8px">值<b>小</b>靠前（与成员 order 相反）</span>
          </el-form-item>
          <el-form-item label="负责人">
            <el-select
              v-model="deptForm.leaders"
              multiple
              filterable
              allow-create
              default-first-option
              placeholder="企微后台维护，此处只读展示"
              style="width: 100%"
              disabled
            />
          </el-form-item>

          <el-divider />

          <p class="dim">
            <span class="field-label">快照时间</span>{{ formatUnix(deptDetail.syncedAt) }}<br />
            <span class="field-label">来源 hash</span>
            <span class="mono">{{ deptDetail.sourceHash ?? '（尚未同步）' }}</span>
          </p>

          <div class="toolbar" style="margin-top: 12px">
            <el-button type="primary" :disabled="!writeEnabled" @click="saveDept">保存</el-button>
            <el-button type="danger" plain :disabled="!writeEnabled || deptDetail.id === 1" @click="removeDept">
              删除部门
            </el-button>
          </div>
        </el-form>
      </div>
    </el-drawer>

    <!-- 成员抽屉 -->
    <el-drawer v-model="memberDrawer.visible" title="成员详情" size="520px" @closed="router.push('/org')">
      <div v-loading="memberDrawer.loading">
        <el-form v-if="memberDetail" label-width="96px">
          <el-form-item label="账号">
            <span class="mono">{{ memberDetail.userId }}</span>
          </el-form-item>
          <el-form-item label="姓名">
            <el-input v-model="memberForm.name" :disabled="!writeEnabled" />
          </el-form-item>
          <el-form-item label="职务">
            <el-input v-model="memberForm.position" :disabled="!writeEnabled" />
          </el-form-item>
          <el-form-item label="性别">
            <el-input v-model="memberForm.gender" placeholder="官方为字符串（1 男 / 2 女）" :disabled="!writeEnabled" />
          </el-form-item>
          <el-form-item label="手机">
            <el-input
              v-model="memberForm.mobile"
              :placeholder="memberDetail.mobile ? '' : '该应用无此字段权限（2022-06-20 起敏感字段停返）'"
              :disabled="!writeEnabled"
            />
          </el-form-item>
          <el-form-item label="邮箱">
            <el-input
              v-model="memberForm.email"
              :placeholder="memberDetail.email ? '' : '该应用无此字段权限（2022-06-20 起敏感字段停返）'"
              :disabled="!writeEnabled"
            />
          </el-form-item>
          <el-form-item label="所属部门">
            <el-select v-model="memberForm.deptIds" multiple filterable style="width: 100%" :disabled="!writeEnabled">
              <el-option v-for="opt in flatDeptOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
            </el-select>
          </el-form-item>
          <el-form-item label="主部门">
            <el-select v-model="memberForm.mainDeptId" clearable style="width: 100%" :disabled="!writeEnabled">
              <el-option
                v-for="deptId in memberForm.deptIds"
                :key="deptId"
                :label="deptNameById.get(deptId) ?? `#${deptId}`"
                :value="deptId"
              />
            </el-select>
          </el-form-item>
          <el-form-item label="部门内负责人">
            <div style="width: 100%">
              <div v-for="deptId in memberForm.deptIds" :key="deptId" style="display: flex; align-items: center; gap: 8px">
                <span style="min-width: 140px">{{ deptNameById.get(deptId) ?? `#${deptId}` }}</span>
                <el-switch
                  :model-value="memberForm.leaderDeptIds.includes(deptId)"
                  :disabled="!writeEnabled"
                  @update:model-value="leaderUpdate(deptId)"
                />
              </div>
            </div>
          </el-form-item>

          <el-divider />

          <p class="dim">
            <span class="field-label">企微状态</span>{{ memberStatusText(memberDetail.status) }}<br />
            <span class="field-label">快照时间</span>{{ formatUnix(memberDetail.syncedAt) }}<br />
            <span class="field-label">来源 hash</span>
            <span class="mono">{{ memberDetail.sourceHash ?? '（尚未同步）' }}</span>
          </p>

          <div class="toolbar" style="margin-top: 12px">
            <el-button type="primary" :disabled="!writeEnabled" @click="saveMember">保存</el-button>
            <el-button type="danger" plain :disabled="!writeEnabled" @click="removeMember">删除成员</el-button>
          </div>
        </el-form>
      </div>
    </el-drawer>

    <!-- 新建部门 -->
    <el-dialog v-model="newDeptDialog.visible" title="新增部门" width="440px">
      <el-form label-width="90px">
        <el-form-item label="名称"><el-input v-model="newDeptForm.name" /></el-form-item>
        <el-form-item label="英文名"><el-input v-model="newDeptForm.nameEn" /></el-form-item>
        <el-form-item label="父部门">
          <el-tree-select
            v-model="newDeptForm.parentId"
            :data="selectTree"
            check-strictly
            :render-after-expand="false"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="排序值">
          <el-input-number v-model="newDeptForm.order" />
          <span class="dim" style="margin-left: 8px">值小靠前</span>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="newDeptDialog.visible = false">取消</el-button>
        <el-button type="primary" :loading="newDeptDialog.saving" @click="submitNewDept">创建</el-button>
      </template>
    </el-dialog>

    <!-- 新建成员 -->
    <el-dialog v-model="newMemberDialog.visible" title="新增成员" width="520px">
      <el-alert
        type="info"
        :closable="false"
        show-icon
        title="部门只能从本地已有部门里选"
        description="官方 user/create 对不存在的部门 id 会自动新建部门（凭空造部门最难收拾），故本 Demo 在提交前逐一校验。"
        style="margin-bottom: 12px"
      />
      <el-form label-width="90px">
        <el-form-item label="账号"><el-input v-model="newMemberForm.userId" placeholder="企业内唯一，1~64 字节" /></el-form-item>
        <el-form-item label="姓名"><el-input v-model="newMemberForm.name" /></el-form-item>
        <el-form-item label="手机"><el-input v-model="newMemberForm.mobile" placeholder="手机与邮箱至少填一个" /></el-form-item>
        <el-form-item label="邮箱"><el-input v-model="newMemberForm.email" /></el-form-item>
        <el-form-item label="职务"><el-input v-model="newMemberForm.position" /></el-form-item>
        <el-form-item label="性别"><el-input v-model="newMemberForm.gender" placeholder="1 男 / 2 女（字符串）" /></el-form-item>
        <el-form-item label="所属部门">
          <el-select v-model="newMemberForm.deptIds" multiple filterable style="width: 100%">
            <el-option v-for="opt in flatDeptOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="主部门">
          <el-select v-model="newMemberForm.mainDeptId" clearable style="width: 100%">
            <el-option
              v-for="deptId in newMemberForm.deptIds"
              :key="deptId"
              :label="deptNameById.get(deptId) ?? `#${deptId}`"
              :value="deptId"
            />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="newMemberDialog.visible = false">取消</el-button>
        <el-button type="primary" :loading="newMemberDialog.saving" @click="submitNewMember">创建</el-button>
      </template>
    </el-dialog>
  </div>
</template>
