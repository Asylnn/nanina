<script lang="ts">
import type Equipment from '@/classes/item/equipment';
import User from '@/classes/user/user';
import GridDisplayComponent from '../Component/GridDisplayComponent.vue';
import WaifuDisplayComponent from '../Component/WaifuDisplayComponent.vue';
import Item from '@/classes/item/item';
import ActivityType from '@/classes/user/activity_type';
import type Waifu from '@/classes/waifu/waifu';
import ActivityProgressComponent from '../Component/ActivityProgressComponent.vue';
import ActivityWaifuPickerComponent from '../Component/ActivityWaifuPickerComponent.vue';

export default {
    name : "ActivitiesPage",
    data(){
        return {
            ActivityType: ActivityType,
            publicPath : import.meta.env.BASE_URL,
            selectedActivity : ActivityType.Help,
            selectedWaifu : null as Waifu | null,
            waifuToView : null as Waifu | null,
            date_milli:0,
            waifuSelectorVisible:false,
            waifuDisplayComponentVisible:false,
        }
    },
    props: {
        user: {
            type: User,
            required: true
        }
    },
    methods:{
        applyTextColor(activity : ActivityType){
            return activity == this.selectedActivity ? "selected" : ""
        },
        openWaifuDisplay(waifu : Waifu)
        {
            
            this.waifuToView = waifu
            this.waifuDisplayComponentVisible = true
        },
        closeWaifuDisplay()
        {
            this.waifuDisplayComponentVisible = false
            this.waifuSelectorVisible = false
            this.selectedWaifu = this.waifuToView
        },
    },
    components:{
        GridDisplayComponent,
        WaifuDisplayComponent,
        ActivityProgressComponent,
        ActivityWaifuPickerComponent,
    },
    computed:{
        
    },
    mounted(){
        setInterval(() => this.date_milli = Date.now(), 1000)
        //This is necessary for the value of date_milli to be updated so the computed value can also be updated
    },
}


</script>
<template>
    
    <div v-if="waifuSelectorVisible">
        <div @click="waifuSelectorVisible = false" class="veil" id="waifuSelectorVeil"></div>
        <GridDisplayComponent id="grid" @show-element="openWaifuDisplay" :elements="user.availableWaifus" :columns=5></GridDisplayComponent>
    </div>
    <div v-if="waifuDisplayComponentVisible">
        <div  @click="waifuDisplayComponentVisible = false" class="veil" id="waifuveil"></div>
        <WaifuDisplayComponent @click="closeWaifuDisplay" :for-pull="false" :for-dungeon="true"  :waifu="waifuToView!" :user="user"></WaifuDisplayComponent>
    </div>
    

    <div>
        <ul id="activityHeader">
            <li :class="applyTextColor(-1) + ' clickable'" @click="selectedActivity = ActivityType.Help">{{$t("activities.help")}}</li>
            <li :class="applyTextColor(0) + ' clickable'" @click="selectedActivity = ActivityType.Cafe">{{$t("activities.cafe")}}</li>
            <li :class="applyTextColor(4) + ' clickable'" @click="selectedActivity = ActivityType.Mining">{{$t("activities.mining")}}</li>
            <li :class="applyTextColor(3) + ' clickable'" @click="selectedActivity = ActivityType.Research">{{$t("activities.research")}}</li>
            <li :class="applyTextColor(2) + ' clickable'" @click="selectedActivity = ActivityType.Crafting" >{{$t("activities.crafting")}}</li>
            <li :class="applyTextColor(1) + ' clickable'" @click="selectedActivity = ActivityType.Exploration" >{{$t("activities.exploration")}}</li>
            <li>{{`${user.activities.length}/${user.maxConcurrentActivities}`}}</li>
        </ul>
    </div>
    <div>
        <div v-if="selectedActivity == ActivityType.Cafe">
            <ActivityWaifuPickerComponent :user="user" :selected-waifu="selectedWaifu"
                v-on:reset-selected-waifu="selectedWaifu = null" 
                v-on:show-waifu-selector="waifuSelectorVisible = true">
            </ActivityWaifuPickerComponent>
        </div>
        <div v-for="activity in user.activities">
            <ActivityProgressComponent v-if="activity.type == selectedActivity" :user="user" :activity="activity"></ActivityProgressComponent>
        </div>
    </div>
</template>

<style lang="css" scoped>
#grid {
    z-index: 150;
    position: sticky;
    top : 10vh;
    right: 0px;
    left : 0px;
    padding:0px;
    margin:10vh 20vw ;
    position:fixed;
    height: 80vh;
    overflow: scroll;
}

#activityHeader {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
    height: 5vh;
    margin: 0 15vw;
    text-align: center;
    cursor: pointer;
}

</style>