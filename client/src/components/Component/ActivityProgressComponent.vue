<script lang="ts">
import User from '@/classes/user/user';
import Item from '@/classes/item/item';
import ActivityType from '@/classes/user/activity_type';
import Activity from '@/classes/user/activity';
import type Waifu from '@/classes/waifu/waifu';
import LootComponent from '../Component/LootComponent.vue';
import { MillisecondsToHourMinuteSecondFormat } from '@/classes/utils';
import ClientResponseType from '@/classes/client_response_type';
export default {
    name : "ActivityProgressComponent",
    data(){
        return {
            ActivityType: ActivityType,
            publicPath : import.meta.env.BASE_URL,
            date_milli:0,
            barWidth:0,
        }
    },
    props: {
        user: {
            type: User,
            required: true
        },
        activity:{
            type:Activity,
            required:true,
        }
    },
    methods:{
        getTimeLeftNumber()
        {
            return MillisecondsToHourMinuteSecondFormat(this.activity.timestamp + this.activity.timeout - this.date_milli)
        },
        getTimeLeft()
        {
            return " width:" + this.barWidth*(this.activity.timestamp + this.activity.timeout - this.date_milli)/this.activity.originalTimeout  + "px";
        },
        getActivityClaim()
        {
            this.SendToServer(ClientResponseType.ClaimActivity, this.activity.id.toString(), this.user.Id)
        },
        cancelActivity()
        {
            this.SendToServer(ClientResponseType.CancelActivity, this.activity.id.toString(), this.user.Id)
        }
    },
    components:{
        LootComponent,
    },
    computed:{
        
    },
    mounted(){
        this.barWidth = (this.$refs as any).getwidth.clientWidth as number
        setInterval(() => this.date_milli = Date.now(), 1000)
        //This is necessary for the value of date_milli to be updated so the computed value can also be updated
    },
}


</script>
<template>
    <div class="flex" id="activity-progress">
        
        <div class="waifuSlot">
            <img :src="`${publicPath}waifu-image/${user.waifus[activity.waifuID].imgPATH}`">
        </div>
        <!-- This is purely used to get the bar's width based on how much space the component takes
             we can't get an element width outside mounted for some reason, and the bar isn't
             processed yet since it's wrapped inside a v-if. We could use a v-show instead.     
             I really would like the width to be dynamic in real time though...               -->                                                                     
        
        <div v-if="! activity.finished" class="flex" id="activityContainer">
            <LootComponent v-if="activity.type == ActivityType.Crafting":loots="activity.loot"></LootComponent>
            <div id="activityBorder" :style="`width:${barWidth}px`">
                <div id="timeleft" :style="getTimeLeft()"></div>
                <span> {{ getTimeLeftNumber() }}</span>
            </div>
            <button class="nnnbutton button" id="activity-cancel-button" @click="cancelActivity()"> {{ $t("activities.cancel") }}</button>
            
        </div>
        <div v-else class="flex" id="activity-loot-display">

                <LootComponent id="activity-loot-component" :loots="activity.loot"></LootComponent>
                <button class="button nnnbutton" id="activity-claim" @click="getActivityClaim()">{{ $t("activities.claim") }}</button>

            
        </div>
        <div v-show="barWidth==0" ref="getwidth" style="width:100%"></div> 
    </div>
</template>

<style lang="css">

.waifuSlot
{
    margin-right: 30px;
}

#loots
{
    margin-left: 30px;
}

#activity-loot-display
{
    width:100%;
    place-content: center;
    place-items: center;
    flex-direction: row;
}

#activity-claim
{
    margin-left: auto;
    margin-right: 20px;
}

#activity-cancel-button
{
    margin-top: 5px;
    margin-bottom: 0px;
}
#activityContainer
{
    display:flex;
    justify-content: center;
    margin-bottom: 20px;
}

#activityBorder
{
    height: 40px;
    margin-top: 10px;
    border-radius: 100px;
    border-style: solid;
    border-color: rgb(200, 41, 200);
    border-width: 5px;                         
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

#activity-progress
{
    flex-direction: row;
}

</style>

