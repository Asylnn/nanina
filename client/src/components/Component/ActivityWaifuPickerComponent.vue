<script lang="ts">
import User from '@/classes/user/user';
import ActivityType from '@/classes/user/activity_type';
import Waifu from '@/classes/waifu/waifu';

export default {
    name : "ActivityWaifuPickerComponent",
    data(){
        return {
            ActivityType: ActivityType,
            publicPath : import.meta.env.BASE_URL,
        }
    },
    props: {
        user: {
            type: User,
            required: true
        },
        selectedWaifu:{
            type:[Waifu, null],
            required:true,
        }
    },
    methods:{
        sendWaifuToCafe()
        {
            this.SendToServer("send waifu to cafe", this.selectedWaifu!.id, this.user.Id)
            this.$emit("reset-selected-waifu")
        },
    },
    emits: ["show-waifu-selector", "reset-selected-waifu"],
}


</script>

<template>
    <p>{{$t("activities.cafe.explanation")}}</p>
    <div v-if="user.activities.length < user.maxConcurrentActivities">
        Add waifu
        <button class="smallbutton nnnbutton" v-if="selectedWaifu != null" @click="sendWaifuToCafe()">{{ $t("activities.sendwaifu") }}</button>
        
        <div class="waifuSlot clickable" @click="$emit('show-waifu-selector')">
            <img :src="`${publicPath}waifu-image/${selectedWaifu?.imgPATH || 'unknown.svg'}`">
        </div>
        
    </div>
</template>

<style lang="css" scoped>

.waifuSlot{
    margin :10px;
    border: 10px;
    border-style: solid;
    border-radius: 20px;
    border-color:rgb(20,20,20);
    width: 15vw;
    height: 15vw;
    overflow: hidden;
}

.waifuSlot img {
    width: 15vw;
    overflow: hidden;
}

</style>