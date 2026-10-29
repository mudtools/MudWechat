import { createRouter, createWebHashHistory } from 'vue-router'

import OrgView from './views/OrgView.vue'
import SyncView from './views/SyncView.vue'
import TemplateDetailView from './views/TemplateDetailView.vue'
import TemplateDiffView from './views/TemplateDiffView.vue'
import TemplateEditView from './views/TemplateEditView.vue'
import TemplatesView from './views/TemplatesView.vue'

/**
 * 路由用 **hash 模式**：后端只有 `UseDefaultFiles + UseStaticFiles`，没有 SPA fallback，
 * HTML5 history 下 `/templates/abc/edit` 刷新会 404。hash 模式不依赖服务端改写，
 * 与「产物直接入库、dotnet run 即可见」的部署形态一致。
 */
const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    { path: '/', redirect: '/org' },
    { path: '/org', name: 'org', component: OrgView },
    // 抽屉是路由化的：刷新/分享能直接定位到某个部门或成员。
    { path: '/org/dept/:id', name: 'org-dept', component: OrgView },
    { path: '/org/member/:userId', name: 'org-member', component: OrgView },
    { path: '/templates', name: 'templates', component: TemplatesView },
    { path: '/templates/:id', name: 'template-detail', component: TemplateDetailView },
    { path: '/templates/:id/diff', name: 'template-diff', component: TemplateDiffView },
    { path: '/templates/:id/edit', name: 'template-edit', component: TemplateEditView },
    { path: '/sync', name: 'sync', component: SyncView },
    { path: '/:pathMatch(.*)*', redirect: '/org' },
  ],
})

export default router
