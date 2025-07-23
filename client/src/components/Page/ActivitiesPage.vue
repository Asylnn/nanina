<script lang="ts">
import type Equipment from '@/classes/item/equipment';
import User from '@/classes/user/user';
import ItemComponent from '../Component/ItemComponent.vue';
import GridDisplayComponent from '../Component/GridDisplayComponent.vue';
import WaifuDisplayComponent from '../Component/WaifuDisplayComponent.vue';
import Item from '@/classes/item/item';
import ActivityType from '@/classes/user/activity_type';
import type Activity from '@/classes/user/activity';
import type Waifu from '@/classes/waifu/waifu';
import LootComponent from '../Component/LootComponent.vue';
import { MillisecondsToHourMinuteSecondFormat } from '@/classes/utils';
export default {
    name : "ActivitiesPage",
    data(){
        return {
            ActivityType: ActivityType,
            publicPath : import.meta.env.BASE_URL,
            activity : "help",
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
        applyTextColor(activity : string){
            return activity == this.activity ? "selected" : ""
        },
        sendWaifuToCafe()
        {
            
            this.SendToServer("send waifu to cafe", this.selectedWaifu!.id, this.user.Id)
            this.selectedWaifu = null
        },
        getTimeLeftNumber(activity : Activity)
        {
            return MillisecondsToHourMinuteSecondFormat(activity.timestamp + activity.timeout - this.date_milli)
        },
        getTimeLeft(activity : Activity)
        {
            return " width:" + 60*(activity.timestamp + activity.timeout - this.date_milli)/activity.timeout  + "vw";
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
        getActivityClaim(activity : Activity)
        {
            this.SendToServer("claim activity", activity.id.toString(), this.user.Id)
        }
    },
    components:{
        GridDisplayComponent,
        WaifuDisplayComponent,
        LootComponent,
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
        <ul id="inventoryHeader">
            <li :class="applyTextColor('help') + ' clickable'" @click="activity = 'help'">{{$t("activities.help")}}</li>
            <li :class="applyTextColor('cafe') + ' clickable'" @click="activity = 'cafe'">{{$t("activities.cafe")}}</li>
            <li :class="applyTextColor('mining') + ' clickable'" @click="activity = 'mining'">{{$t("activities.mining")}}</li>
            <li :class="applyTextColor('research') + ' clickable'" @click="activity = 'research'">{{$t("activities.research")}}</li>
            <li :class="applyTextColor('crafting') + ' clickable'" @click="activity = 'crafting'" >{{$t("activities.crafting")}}</li>
            <li :class="applyTextColor('exploration') + ' clickable'" @click="activity = 'exploration'" >{{$t("activities.exploration")}}</li>
            <li>{{`${user.activities.length}/${user.maxConcurrentActivities}`}}</li>
        </ul>
    </div>
    <div>
        <div v-if="activity == 'cafe'">
            <p>{{$t("activities.cafe.explanation")}}</p>
            <div v-if="user.activities.length < user.maxConcurrentActivities">
                Add waifu
                <button class="smallbutton nnnbutton" v-if="selectedWaifu != null" @click="sendWaifuToCafe()">{{ $t("activities.sendwaifu") }}</button>
                
                <div class="waifuSlot clickable" @click="waifuSelectorVisible = true">
                    <img :src="`${publicPath}waifu-image/${selectedWaifu?.imgPATH || 'unknown.svg'}`">
                </div>
                
            </div>
            <div v-for="activity in user.activities">
                <div v-if="activity.type == ActivityType.Cafe">
                    <div class="waifuSlot">
                        <img :src="`${publicPath}waifu-image/${user.waifus.find(waifu => waifu.id == activity.waifuID)!.imgPATH}`">
                    </div>
                    <div v-if="! activity.finished" id="activityContainer">
                        <div id="activityBorder">
                            <div id="timeleft" :style="getTimeLeft(activity)">
                            </div>
                            <span> {{ getTimeLeftNumber(activity) }}</span>
                        </div>
                    </div>
                    <div v-else>
                        activity finished
                        <LootComponent :loots="activity.loot"></LootComponent>
                        <button class="smallbutton nnnbutton" @click="getActivityClaim(activity)">{{ $t("activities.claim") }}</button>
                    </div>
                </div>
            </div>
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

#inventoryHeader {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
    height: 5vh;
    margin: 0 15vw;
    text-align: center;
    cursor: pointer;
}

#activityContainer
{
    display:flex;
    height: 60px;
    justify-content: center;
    margin-bottom: 20px;
}

#activityBorder
{
    height: 40px;
    width: 60vw;
    border-radius: 100px;
    border-style: solid;
    border-color: rgb(200, 41, 200);
    border-width: 8px;                         
    display:flex;
    align-items: center;
}

#activityBorder span
{
    position:absolute;
    left:50%;
    transform: translateX(-50%);
    font-size: larger;
}

#timeleft
{
    height: 40px;
   
    justify-self: end;
    border-radius: 100px;
    border-style: none;
    border-width: 10px;
    background-color: red;
}


</style>