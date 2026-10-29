<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { Refresh } from '@element-plus/icons-vue'

import { useHealth } from './composables/useHealth'

const route = useRoute()
const { health, writeEnabled, refresh } = useHealth()

const activeMenu = computed(() => (route.path.startsWith('/templates') ? '/templates' : route.path.startsWith('/sync') ? '/sync' : '/org'))

const storeRows = computed(() => {
  const tables = health.value?.config.store.tables ?? {}
  const preferred = [
    'org_department',
    'org_member',
    'org_member_department',
    'oa_template',
    'oa_template_version',
    'oa_template_registry',
  ]
  return preferred.filter((name) => name in tables).map((name) => `${name}=${tables[name]}`)
})

const tableTotal = computed<number>(() => {
  const tables: Record<string, number> = health.value?.config.store.tables ?? {}
  return Object.values(tables).reduce((sum: number, value: number) => sum + value, 0)
})
</script>

<template>
  <el-container class="app-shell">
    <el-header class="app-header" height="56px">
      <span class="app-header__title">企业微信 · 组织机构与审批模板</span>

      <el-tag v-for="app in health?.config.apps ?? []" :key="app.appKey" size="small" type="info">
        {{ app.appKey }} {{ app.secretTail }}
      </el-tag>

      <el-tag size="small" :type="writeEnabled ? 'warning' : 'success'">
        写闸：{{ writeEnabled ? '已放行（会写真企业）' : '关闭' }}
      </el-tag>

      <el-tag size="small" :type="health?.config.callback.enabled ? 'success' : 'info'">
        回调：{{ health?.config.callback.enabled ? '已装配' : '未装配' }}
      </el-tag>

      <span class="app-header__spacer" />
      <el-button :icon="Refresh" size="small" @click="refresh">刷新状态</el-button>
    </el-header>

    <el-container class="app-body">
      <el-aside width="210px" class="app-aside">
        <el-menu :default-active="activeMenu" router>
          <el-menu-item index="/org">组织机构</el-menu-item>
          <el-menu-item index="/templates">审批模板</el-menu-item>
          <el-menu-item index="/sync">同步中心</el-menu-item>
        </el-menu>

        <div class="app-aside__note">
          <p><b>本地台账</b>：{{ tableTotal }} 行（9 张表）</p>
          <p v-if="storeRows.length">已用：{{ storeRows.join(' · ') }}</p>
          <p class="app-aside__dim">企业微信是组织机构的唯一权威源；本地 SQLite 只是镜像台账。</p>
        </div>
      </el-aside>

      <el-main class="app-main">
        <el-alert
          v-if="!writeEnabled"
          type="info"
          :closable="false"
          show-icon
          title="写操作端点未映射"
          description="设置 MUDWECHAT_DEMO_ALLOW_WRITE=1 后重启，才会映射「建部门 / 建成员 / 模板创建与覆盖式更新」等端点。本页所有写按钮已按 /healthz 的 writeGate 提前置灰。"
          style="margin-bottom: 12px"
        />
        <el-alert
          v-else
          type="warning"
          :closable="false"
          show-icon
          title="当前会向真实企业写入数据"
          description="删除部门/成员不可逆；update_template 是覆盖式全量替换（漏传控件即删除）。请确认指向测试企业。"
          style="margin-bottom: 12px"
        />

        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>
