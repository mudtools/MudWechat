import { createApp } from 'vue'
import ElementPlus from 'element-plus'
import zhCn from 'element-plus/es/locale/lang/zh-cn'
import * as ElementPlusIcons from '@element-plus/icons-vue'
import 'element-plus/dist/index.css'

import App from './App.vue'
import './styles/main.css'

// Element Plus 全量引入：按需引入对 Demo 无收益，且多一层与 Rolldown 内核的兼容风险（方案 §6.1）。
const app = createApp(App)

for (const [name, component] of Object.entries(ElementPlusIcons)) {
  app.component(name, component)
}

app.use(ElementPlus, { locale: zhCn })
app.mount('#app')
