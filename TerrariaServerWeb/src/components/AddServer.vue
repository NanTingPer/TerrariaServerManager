<template>
    <table>
        <tr>
            <td><el-text>服务器名称</el-text> </td>
            <td><el-input v-model="serverName" style="width: auto;"></el-input></td>
        </tr>        
        <tr>
            <td><el-text>世界大小</el-text></td>
            <td>
                <el-select v-model="worldSize" placeholder="Select">
                    <el-option v-for="item in siezOptions" :key="item.value" :label="item.lable" :value="item.value"></el-option>
                </el-select>
            </td>
        </tr>        
        <tr>
            <td><el-text>服务器端口</el-text></td>
            <td><el-input type="number" v-model="serverPort" style="width: auto;"></el-input></td>
        </tr>        
        <tr>
            <td><el-text>服务器最大人数</el-text></td>
            <td><el-input type="number" v-model="serverMaxPlayer" style="width: auto;"></el-input></td>
        </tr>        
        <tr>
            <td><el-text>服务器密码</el-text></td>
            <td><el-input v-model="serverPasswd" style="width: auto;"></el-input></td>
        </tr>        
        <tr>
            <td><el-text>服务器监听IP</el-text></td>
            <td><el-input v-model="serverIP" style="width: auto;"></el-input></td>
        </tr>      
        <tr>
            <td><el-text>世界名称</el-text></td>
            <td><el-input v-model="worldName" style="width: auto;"></el-input></td>
        </tr> 
        <tr>
            <td><el-text>世界种子</el-text></td>
            <td><el-input v-model="worldSeed" style="width: auto;"></el-input></td>
        </tr>        
        <tr>
            <td><el-text>世界邪恶</el-text></td>
            <td>
                <el-select v-model="worldEvil" placeholder="Select">
                    <el-option v-for="item in evilOptions" :key="item.value" :label="item.lable" :value="item.value">
                    </el-option>
                </el-select>
            </td>
        </tr>
        <tr>
            <td><el-button @click="Submit">确定</el-button></td>
            <td><el-button @click="Exit">取消</el-button></td>
        </tr>
    </table>
</template>

<script lang="ts" setup>
import axios from 'axios'
const emit = defineEmits(['submit'])
async function Submit(){
    const subObj : AppendServerRequest = {
        Name : serverName.value,
        options : {
            autoCreate : worldSize.value.toString(),
            port : serverPort.value.toString(),
            maxPlayers : serverMaxPlayer.value.toString(),
            password : serverPasswd.value.toString(),
            ip : serverIP.value.toString(),
            seed : worldSeed.value,
            evil : worldEvil.value.toString(),
            worldName : worldName.value
        }
    }
    await axios.post('/server/add', subObj).then().catch().finally(() => emit('submit'))
    
}

function Exit(){
    emit('submit')
}

import { ref } from 'vue'
import type { AppendServerRequest } from '../types/AppendServerRequest'

const serverName = ref("")
const serverPort = ref("7777")
const serverMaxPlayer = ref("16")
const serverPasswd = ref("")
const serverIP = ref("0.0.0.0")
const worldSize = ref(1)
const worldEvil = ref(1)
const worldName = ref("")
const worldSeed = ref("")

const evilOptions = [
    {
        value : 1,
        lable : "随机"
    },
    {
        value : 2,
        lable : "腐坏"
    },
    {
        value : 3,
        lable : "血腥"
    }
]
const siezOptions = [
    {
        value : 1,
        lable : "小"
    },
    {
        value : 2,
        lable : "中"
    },
    {
        value : 3,
        lable : "大"
    },
]
</script>