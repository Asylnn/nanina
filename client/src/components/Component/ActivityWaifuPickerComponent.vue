<script lang="ts">
import User from '@/classes/user/user';
import ActivityType from '@/classes/user/activity_type';
import Waifu from '@/classes/waifu/waifu';
import ClientResponseType from '@/classes/client_response_type';
import ActivityProgressComponent from './ActivityProgressComponent.vue';

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
           return ActivityType[this.activityType].toLowerCase()
        }
    },
    components:{
        ActivityProgressComponent
    },
    emits: ["show-waifu-selector", "reset-selected-waifu", "start-crafting-activity"],
}


</script>

<template>
    <div class="margins">
        <p v-if="activityType != ActivityType.Crafting">{{$t(`activities.${getActivityName()}.overview`)}}</p>
        <div v-if="user.activities.length < user.maxConcurrentActivities">
            <div class="flex" id="waifu-selector">
                <div class="waifuSlot clickable" @click="$emit('show-waifu-selector')">
                    <img :src="`${publicPath}waifu-image/${selectedWaifu?.imgPATH || 'unknown.svg'}`">
                </div>
                <button class="button nnnbutton" id="startbutton" v-if="selectedWaifu != null && showButton" @click="sendWaifuToActivity()">{{ $t("activities.start") }}</button>
            </div>
            <div v-for="activity in user.activities"> 
                <ActivityProgressComponent :user="user" :activity="activity"
                    v-if="activity.type == activityType && (activityType == ActivityType.Cafe || activityType == ActivityType.Mining || activityType == ActivityType.Exploration)" >
                </ActivityProgressComponent>
            </div>
        </div>
    </div>
</template>

<style lang="css">

.waifuSlot{
    margin :0px 10px;
    border: 10px;
    border-style: solid;
    border-radius: 20px;
    border-color:rgb(20,20,20);
    width: 8vw;
    height: 8vw;
    overflow: hidden;
    flex-shrink: 0; /*For Activity Progress Component's waifuSlot*/ 
}

.waifuSlot img {
    width: 8vw;
    overflow: hidden;
}

#waifu-selector
{
    flex-direction: row;
    margin-left: 4vw;
    margin-top:10px;
    margin-bottom: 20px;
}

.startbutton
{
    margin-left: 6vw;
}

</style>