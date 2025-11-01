<template>
    <div v-if="divIsShow" class="addServer">
        <AddServer @submit="SetDivIsShow"></AddServer>
    </div>
    <div v-if="isShowLog" class="log">
        <Log :id="viewId" @exit="SetViewLogIsShow"></Log>
    </div>
    <el-button @click="SetDivIsShow">添加</el-button>
    <el-table :data="tableData" style="width: 100%">
    <el-table-column prop="id" label="唯一标识" style="width: auto;"/>
    <el-table-column prop="name" label="名称" style="width: auto;"/>
    <el-table-column label="服务器配置">
        <el-table-column prop="options.autoCreate" label="世界大小" :formatter="WorldSizeFormatter"></el-table-column>
        <el-table-column prop="options.port" label="服务器端口"></el-table-column>
        <el-table-column prop="options.maxPlayers" label="服务器最大人数"></el-table-column>
        <el-table-column prop="options.password" label="服务器密码"></el-table-column>
        <el-table-column prop="options.ip" label="服务器监听IP"></el-table-column>
        <el-table-column prop="options.seed" label="世界种子"></el-table-column>
        <el-table-column prop="options.evil" label="世界邪恶" :formatter="WroldEvilFormatter"></el-table-column>
        <el-table-column prop="options.worldName" label="世界名称"></el-table-column>
    </el-table-column>
    <el-table-column label="操作">
        <template #default="value">
            <el-button @click="Delete(value.row)">删除</el-button>
        </template>
    </el-table-column>    
    <el-table-column label="操作">
        <template #default="value">
            <el-button @click="ViewLog(value.row)">查看日志</el-button>
        </template>
    </el-table-column>
    </el-table>
</template>

<script lang="ts" setup>
import AddServer from './AddServer.vue'
import Log from './Log.vue'
import axios from 'axios'
import { ref, onMounted } from 'vue'
import { type ViewServer } from '../types/ViewServer'
import type { TableColumnCtx } from 'element-plus';

const tableData = ref<ViewServer[]>()
const divIsShow = ref<boolean>(false)
const isShowLog = ref<boolean>(false)
const viewId = ref<number>()

async function GetServerList(){
    await axios.post("/server/list", {}).then(response => {
        if(response.status == 200){
            tableData.value = response.data as ViewServer[]
            console.log(response.data)
        }
    })
}

onMounted(GetServerList);

function WorldSizeFormatter(_row: any, _column: TableColumnCtx<any>, cellValue: any, _index: number){
    if(cellValue == 1)
        return "小"
    else if(cellValue == 2)
        return "中"
    else 
        return "大"
}

function WroldEvilFormatter(_row: any, _column: TableColumnCtx<any>, cellValue: any, _index: number){
    if(cellValue == 1)
        return "随机"
    else if(cellValue == 2)
        return "腐坏"
    else if(cellValue == 3)
        return "猩红"
}

async function SetDivIsShow(){
    await GetServerList();
    divIsShow.value = !divIsShow.value;
}

function ViewLog(row : ViewServer){
    viewId.value = row.id
    SetViewLogIsShow()
}

function SetViewLogIsShow(){
    isShowLog.value = !isShowLog.value
}

async function Delete(row : ViewServer){
    await axios.post('server/delete', row.id, {
        headers: {
            "Content-Type": "application/json"
        }
    }).catch(e => console.log(e))
    await GetServerList();
}
</script>

<style scoped>
.addServer {
    position: absolute;
    top: 0%;
    left: 0%;
    float: left;
    background-color:antiquewhite;
    height: 100%;
    width: 100%;
    z-index: 99;

    display: flex;
    justify-content: center;
    align-items: center;
}

.log {
    position: absolute;
    top: 0%;
    left: 0%;
    float: left;
    background-color:antiquewhite;
    height: 100%;
    width: 100%;
    z-index: 99;

    display: flex;
    justify-content: center;
    align-items: center;
}
</style>

<style>
.cell {
    display: flex;
    text-align: center;
    justify-content: center;
    align-items: center;
}
</style>