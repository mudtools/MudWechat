<script setup lang="ts">
import { ref, watch } from 'vue'

import { listMaterials } from '../api/articles'
import type { MaterialSummary } from '../api/types'
import { formatUnix, tail } from '../utils/format'
import { presentError } from '../utils/notify'

const visible = defineModel<boolean>('visible', { required: true })

const props = defineProps<{ mode: 'cover' | 'content' }>()
const emit = defineEmits<{ choose: [material: MaterialSummary] }>()

const pageSize = 20
const items = ref<MaterialSummary[]>([])
const total = ref(0)
const currentPage = ref(1)
const loading = ref(false)
const hint = ref('')

async function load() {
  loading.value = true
  try {
    const page = await listMaterials((currentPage.value - 1) * pageSize, pageSize)
    items.value = page.items
    total.value = page.totalCount
    hint.value = page.hint ?? ''
  } catch (reason) {
    items.value = []
    total.value = 0
    presentError(reason)
  } finally {
    loading.value = false
  }
}

watch(visible, (opened) => {
  if (opened) {
    void load()
  }
})
</script>

<template>
  <el-drawer
    :model-value="visible"
    :title="props.mode === 'cover' ? '选择封面（永久图片素材）' : '选择正文插图'"
    size="60%"
    @update:model-value="visible = $event as boolean"
  >
    <el-alert
      type="info"
      :closable="false"
      show-icon
      :title="props.mode === 'cover'
        ? 'news 图文必须有 thumb_media_id：这里选的是账号已有的永久图片素材'
        : '正文图片只能用素材库里的 url —— 站外图片会被官方过滤'"
    />

    <div class="panel__toolbar" style="margin-top: 10px">
      <el-button size="small" :loading="loading" @click="load">刷新</el-button>
      <el-tag size="small" type="info">共 {{ total }} 张</el-tag>
      <span style="flex: 1" />
      <el-pagination
        v-model:current-page="currentPage"
        :page-size="pageSize"
        :total="total"
        layout="prev, next"
        size="small"
        @current-change="load"
      />
    </div>

    <div v-loading="loading">
      <div v-if="items.length" class="material-grid">
        <div
          v-for="item in items"
          :key="item.mediaId"
          class="material-card"
          @click="emit('choose', item)"
        >
          <el-image :src="item.url ?? ''" class="material-card__image" fit="contain" lazy />
          <div class="material-card__name" :title="item.name ?? ''">{{ item.name || '(未命名)' }}</div>
          <el-text size="small" type="info">
            {{ props.mode === 'cover' ? tail(item.mediaId, 8) : '插入此图' }}
          </el-text>
          <div>
            <el-text size="small">{{ formatUnix(item.updateTime) }}</el-text>
          </div>
        </div>
      </div>
      <el-empty v-else-if="!loading" description="素材库里没有图片素材" :image-size="80" />
    </div>

    <el-text size="small" type="info">{{ hint }}</el-text>
  </el-drawer>
</template>
