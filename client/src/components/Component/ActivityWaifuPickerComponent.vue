<script lang="ts">
import User from '@/classes/user/user';
import ActivityType from '@/classes/user/activity_type';
import Waifu from '@/classes/waifu/waifu';
import ClientResponseType from '@/classes/client_response_type';

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
        },
        activityType:{
            type:Number,
            required:true,
        },
        showButton:{
            type:Boolean,
            required:false,
            default:true,
        }
    },
    methods:{
        sendWaifuToActivity()
        {
            if(this.activityType != ActivityType.Crafting)
                this.SendToServer(ClientResponseType.SendWaifuToActivity, JSON.stringify({"waifuID": this.selectedWaifu!.id, "activityType":this.activityType}), this.user.Id)
            
            else
                this.$emit("start-crafting-activity")

            this.$emit("reset-selected-waifu")
            
        },
        getActivityName()
        {
            switch(this.activityType)
            {
                case ActivityType.Cafe:
                    return "cafe"
                case ActivityType.Crafting:
                    return "crafting"
                case ActivityType.Exploration:
                    return "exploration"
                case ActivityType.Research:
                    return "research"
                case ActivityType.Mining:
                    return "mining"
            }
        }
    },
    emits: ["show-waifu-selector", "reset-selected-waifu", "start-crafting-activity"],
}


</script>

<template>
    <p v-if="activityType != ActivityType.Crafting">{{$t(`activities.${getActivityName()}.overview`)}}</p>
    <div v-if="user.activities.length < user.maxConcurrentActivities">
        <button class="smallbutton nnnbutton" v-if="selectedWaifu != null && showButton" @click="sendWaifuToActivity()">{{ $t("activities.start") }}</button>
        
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
    width: 8vw;
    height: 8vw;
    overflow: hidden;
}

.waifuSlot img {
    width: 8vw;
    overflow: hidden;
}

</style>