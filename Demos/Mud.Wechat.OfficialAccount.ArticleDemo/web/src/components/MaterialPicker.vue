<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{ thumbMediaId: string; previewUrl: string }>()
const emit = defineEmits<{ open: [] }>()

const idTail = computed(() =>
  props.thumbMediaId ? `…${props.thumbMediaId.slice(-8)}` : '未选',
)
</script>

<template>
  <div style="display: flex; gap: 12px; align-items: flex-start">
    <el-image
      v-if="props.previewUrl"
      :src="props.previewUrl"
      class="editor-cover__thumb"
      fit="contain"
      :preview-src-list="[props.previewUrl]"
    />
    <div>
      <div>
        <el-text size="small">
          封面 media_id：<code>{{ idTail }}</code>
        </el-text>
      </div>
      <el-button size="small" type="primary" plain @click="emit('open')">
        {{ props.thumbMediaId ? '换一张' : '从素材库选封面' }}
      </el-button>
      <div>
        <el-text size="small" type="info">
          本 Demo 无上传端点（见 README「已知边界」）：素材需先在公众平台官网素材库上传。
        </el-text>
      </div>
    </div>
  </div>
</template>
