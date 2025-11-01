<template>
    <el-text truncated> {{ logValue }} </el-text>
    <br></br>
    <el-button @click="Exit">退出</el-button>
</template>

<script setup lang="ts">
import { onMounted, ref} from 'vue'
const logValue = ref("")
const props = defineProps({
  id: Number,
})

const emit = defineEmits(['exit'])

import axios from 'axios'
onMounted(async () => {
    await axios.post('server/log', props.id, {
        headers: {
            "Content-Type": "application/json"
        }
    })
    .then(res => logValue.value = res.data)
    .catch(e => console.log(e))
})

function Exit(){
    emit('exit')
}
</script>