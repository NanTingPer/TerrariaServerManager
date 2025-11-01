<script setup lang="ts">
import router from './router';

let upRouteName : string
function GotoRoute(route : string){
    // window.location.pathname=route;
    history.pushState(null, '', route);
    if(router.hasRoute(upRouteName)){
        router.removeRoute(upRouteName);
    }
    router.push(route);
    upRouteName = route;
}
</script>

<template>
      <el-container>
        <el-container>
            <el-aside class="aside">
                <el-menu>
                    <el-menu-item @click="GotoRoute('/')" index="1"> 首页 </el-menu-item>
                    <el-menu-item @click="GotoRoute('/')" index="1"> 后台 </el-menu-item>
                    <el-menu-item @click="GotoRoute('/')" index="1"> 关于 </el-menu-item>
                </el-menu>
            </el-aside>

            <el-main class="main">
                <RouterView></RouterView>
            </el-main>
        </el-container>
    </el-container>
</template>

<style scoped>
.el-menu--vertical {
  height: 100%;
}
.el-aside {
  height: 100%;
}
</style>
