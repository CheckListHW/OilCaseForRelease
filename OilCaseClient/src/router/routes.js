const routes = [
  {
    path: '',
    component: () => import('layouts/FirstLayout.vue'),
    children: [
      { path: '/', component: () => import('pages/Index.vue') }
    ]
  },
  {
    path: '/LoginPage',
    component: () => import('layouts/FirstLayout.vue'),
    children: [
      { path: '', component: () => import('pages/LoginReg/LoginPage.vue') }
    ]
  },
  {
    path: '/legend',
    component: () => import('layouts/BodyLayout.vue'),
    children: [
      { path: '', component: () => import('pages/legend.vue') }
    ]
  },
  {
    path: '/tasks',
    component: () => import('layouts/BodyLayout.vue'),
    children: [
      { path: '', component: () => import('pages/tasks.vue') }
    ]
  },
  {
    path: '/result',
    component: () => import('layouts/BodyLayout.vue'),
    children: [
      { path: '', component: () => import('pages/result.vue') }
    ]
  },
  {
    path: '/endStep',
    component: () => import('layouts/BodyLayout.vue'),
    children: [
      { path: '', component: () => import('pages/endStep.vue') }
    ]
  },
  {
    path: '/3DScene',
    component: () => import('layouts/BodyLayout.vue'),
    children: [
      { path: '', component: () => import('pages/3DScene.vue') }
    ]
  },

  {
    path: '/showDictMode',
    component: () => import('layouts/BodyLayout.vue'),
    children: [
      { path: '', component: () => import('pages/trash/showDictMode.vue') }
    ]
  },
  {
    path: '/showDictUsers',
    component: () => import('layouts/BodyLayout.vue'),
    children: [
      { path: '', component: () => import('pages/trash/showDictUsers.vue') }
    ],
  },
  {
    path: '/showUserLog',
    component: () => import('layouts/BodyLayout.vue'),
    children: [
      { path: '', component: () => import('pages/trash/showUserLog.vue') }
    ]
  },

  {
    path: '/editUserPage/:emplid',
    component: () => import('layouts/FirstLayout.vue'),
    children: [
      { path: '', component: () => import('pages/trash/editUserPage.vue') }
    ]
  },
  {
    path: '/surfcheck',
    component: () => import('layouts/FirstLayout.vue'),
    children: [
      { path: '', component: () => import('pages/trash/surfCheckPage.vue') }
    ]
  },
]

if (process.env.MODE !== 'ssr') {
  routes.push({
    path: '*',
    component: () => import('pages/error404.vue')
  })
}

export default routes
