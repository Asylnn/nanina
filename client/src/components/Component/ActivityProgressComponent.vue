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
            return " width:" + 60*(this.activity.timestamp + this.activity.timeout - this.date_milli)/this.activity.originalTimeout  + "vw";
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
        setInterval(() => this.date_milli = Date.now(), 1000)
        //This is necessary for the value of date_milli to be updated so the computed value can also be updated
    },
}


</script>
<template>
    <div class="waifuSlot">
        <img :src="`${publicPath}waifu-image/${user.waifus.find(waifu => waifu.id == activity.waifuID)!.imgPATH}`">
    </div>
    <div v-if="! activity.finished" id="activityContainer">
        <div id="activityBorder">
            <div id="timeleft" :style="getTimeLeft()">
            </div>
            <span> {{ getTimeLeftNumber() }}</span>
        </div>
        <button class="nnnbutton smallbutton" @click="cancelActivity()"> {{ "cancel" }}</button>
        <LootComponent v-if="activity.type == ActivityType.Crafting":loots="activity.loot"></LootComponent>
    </div>
    <div v-else>
        activity finished
        <LootComponent :loots="activity.loot"></LootComponent>
        <button class="smallbutton nnnbutton" @click="getActivityClaim()">{{ $t("activities.claim") }}</button>
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

