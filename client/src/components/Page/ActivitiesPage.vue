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
import ResearchNode from '@/classes/research/research_nodes';
import ResearchPage from './ResearchPage.vue';
import CraftingPage from './CraftingPage.vue';
import Craft from '@/classes/crafting/craft';
import ClientResponseType from '@/classes/client_response_type';
import ContinuousFightPage from './ContinuousFightPage.vue';
import type Dictionary from '@/classes/dictionary';
import type { PropType } from 'vue';
import { Websocket, WebsocketEvent } from 'websocket-ts';
import type WebSocketResponse from '@/classes/web_socket_response';
import ServerResponseType from '@/classes/server_response_type';
import type Activity from '@/classes/user/activity';
import ActivitiesHelpComponent from './ActivitiesHelpComponent.vue';


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
            listener: (() => {}) as (i: Websocket, ev: MessageEvent) => void,
        }
    },
    props: {
        user: {
            type: User,
            required: true
        },
        researchNodes:{
            type:Object as PropType<Dictionary<ResearchNode>>,
            required:true,
        },
        craftingRecipes:{
            type:Object as PropType<Dictionary<Craft>>,
            required:true,
        },
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
        selectWaifu()
        {
            this.waifuDisplayComponentVisible = false
            this.waifuSelectorVisible = false
            this.selectedWaifu = this.waifuToView
        },
        showWaifuSelector()
        {
            this.waifuSelectorVisible = true;
            this.selectedWaifu = null;
        },
    },
    components:{
        GridDisplayComponent,
        WaifuDisplayComponent,
        ActivityProgressComponent,
        ActivityWaifuPickerComponent,
        ResearchPage,
        CraftingPage,
        ContinuousFightPage,
        ActivitiesHelpComponent,
    },
    computed:{
        
    },
    mounted()
    {
        setInterval(() => this.date_milli = Date.now(), 1000)
        //This is necessary for the value of date_milli to be updated so the computed value can also be updated
        this.listener = (i: Websocket, ev: MessageEvent) => {
			var res : WebSocketResponse = JSON.parse(ev.data)
			switch (res.type) 
            {
                case ServerResponseType.ConfirmActivity:
                    let activity = JSON.parse(res.data) as Activity
                    this.user.activities.push(activity)
                    this.user.waifus[activity.waifuID].isDoingSomething = true
                    break;
                case ServerResponseType.ConfirmCancelActivity:
                    let index = this.user.activities.findIndex(activity => activity.id == res.data)
                    this.user.activities.splice(index, 1)
                    break;
                case ServerResponseType.ConfirmActivityClaim:
                    let claimedActivity = JSON.parse(res.data) as Activity
                    var waifu = this.user.waifus[claimedActivity.waifuID];
                    waifu.isDoingSomething = false;
                    let indexToDelete = this.user.activities.findIndex(activity => activity.id == claimedActivity.id)
                    this.user.activities.splice(indexToDelete, 1)
                    break;
            }
        }
        //@ts-ignore
        this.ws.addEventListener(WebsocketEvent.message, this.listener);
    },
    unmounted()
    {
        //@ts-ignore
        this.ws.removeEventListener(WebsocketEvent.message, this.listener)
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
        <WaifuDisplayComponent @click="selectWaifu" :for-pull="false" :for-dungeon="true"  :waifu="waifuToView!" :user="user"
            @exit="waifuDisplayComponentVisible = false">
        </WaifuDisplayComponent>
    </div>
    

    <div>
        <ul id="activityHeader">
            <li :class="applyTextColor(-2) + ' clickable'" @click="selectedActivity = ActivityType.Help">{{$t("activities.help.submenu")}}</li>
            <li :class="applyTextColor(0) + ' clickable'" @click="selectedActivity = ActivityType.Cafe">{{$t("activities.cafe.submenu")}}</li>
            <li :class="applyTextColor(4) + ' clickable'" @click="selectedActivity = ActivityType.Mining">{{$t("activities.mining.submenu")}}</li>
            <li :class="applyTextColor(3) + ' clickable'" @click="selectedActivity = ActivityType.Research">{{$t("activities.research.submenu")}}</li>
            <li :class="applyTextColor(2) + ' clickable'" @click="selectedActivity = ActivityType.Crafting" >{{$t("activities.crafting.submenu")}}</li>
            <li :class="applyTextColor(1) + ' clickable'" @click="selectedActivity = ActivityType.Exploration" >{{$t("activities.exploration.submenu")}}</li>
            <li :class="applyTextColor(-1) + ' clickable'" @click="selectedActivity = ActivityType.ContinousFight" >{{$t("activities.fight.submenu")}}</li>
            <li>{{`${user.activities.length}/${user.maxConcurrentActivities}`}}</li>
        </ul>
    </div>
    <div>
        <div v-if="selectedActivity == ActivityType.Help">
            <ActivitiesHelpComponent></ActivitiesHelpComponent>
            
        </div>
        <div v-if="selectedActivity == ActivityType.Cafe || selectedActivity == ActivityType.Mining || selectedActivity == ActivityType.Exploration">
            <ActivityWaifuPickerComponent :user="user" :selected-waifu="selectedWaifu" :activity-type="selectedActivity"
                v-on:reset-selected-waifu="selectedWaifu = null" 
                v-on:show-waifu-selector="showWaifuSelector()">
            </ActivityWaifuPickerComponent>
        </div>
        <div v-else-if="selectedActivity == ActivityType.Research">
            <ResearchPage :research-nodes="Object.values(researchNodes)" :user="user" :selected-waifu="selectedWaifu"
                v-on:show-waifu-selector="showWaifuSelector()"
                v-on:reset-selected-waifu="selectedWaifu = null">
                
            </ResearchPage>
        </div>
        <div v-else-if="selectedActivity == ActivityType.Crafting">
            <CraftingPage :user="user" :selected-waifu="selectedWaifu" :crafting-recipes="Object.values(craftingRecipes)"
                v-on:reset-selected-waifu="selectedWaifu = null" 
                v-on:show-waifu-selector="showWaifuSelector()">

            </CraftingPage>
        </div>
        <div v-if="selectedActivity == ActivityType.ContinousFight">
            <ContinuousFightPage :user="user"></ContinuousFightPage>
        </div>
        <div v-for="activity in user.activities"> 
            <ActivityProgressComponent :user="user" :activity="activity"
                v-if="activity.type == selectedActivity && (selectedActivity == ActivityType.Cafe || selectedActivity == ActivityType.Mining || selectedActivity == ActivityType.Exploration)" >
            </ActivityProgressComponent>
        </div>
    </div>
</template>

<style lang="css" scoped>
#grid {
    z-index: 50;
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
    grid-template-columns: 1fr 1fr 1fr 1fr 1fr 1fr 1fr 1fr;
    height: 5vh;
    margin: 0 15vw;
    text-align: center;
    cursor: pointer;
}

#waifuSelectorVeil{
    z-index: 40;
}

#waifuveil{
    z-index: 60;
}

</style>