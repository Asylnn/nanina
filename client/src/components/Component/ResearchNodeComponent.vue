<script lang="ts">
import ActivityType from '@/classes/user/activity_type';
import ResearchNode from '@/classes/research/research_nodes';
import User from '@/classes/user/user';
import Waifu from '@/classes/waifu/waifu';
import { MillisecondsToHourMinuteSecondFormat } from '@/classes/utils';
import type Activity from '@/classes/user/activity';
import ClientResponseType from '@/classes/client_response_type';

export default {
    name: "ResearchNodeComponent",
    data() {
        return {
            ActivityType: ActivityType,
            publicPath: import.meta.env.BASE_URL,
            date_milli: 0,
        }
    },
    props: {
        user: {
            type: User,
            required: true
        },
        researchNode: {
            type: ResearchNode,
            required: true,
        },
        selectedWaifu: {
            type: [Waifu, null],
            required: true,
        }
    },
    methods: {
        getActivityClaim()
        {
            this.SendToServer(ClientResponseType.ClaimActivity, this.activity!.id.toString(), this.user.Id)
            this.$emit("reset-selected-waifu")
        },
        getTimeLeftNumber(activity: Activity) {
            return MillisecondsToHourMinuteSecondFormat(activity.timestamp + activity.timeout - this.date_milli)
        },
        getTimeLeft(activity: Activity) {
            return " width:" + 340 * (activity.timestamp + activity.timeout - this.date_milli) / activity.originalTimeout + "px";
        },
        getBackgroundColor() {
            var style = "background-color:"
            if(this.activity != null)
            {
                style += "lightblue;"
                style += "cursor: pointer;"
            }
            if (Object.keys(this.user.completedResearches).some(id => id == this.researchNode.id) && !this.researchNode.infinite) {
                style += "green;"
            }

            else if (this.researchNode.requirements.every(researchID => this.user.isResearchDone(researchID))) {
                style += "yellow;"
                style += "cursor: pointer;"
            }
            else {
                style += "red;"
                style += "cursor: not-allowed"
            }
            return style
        },
        sendWaifuToResearch() {
            var u = {
                waifuID: this.selectedWaifu!.id,
                researchID: this.researchNode.id,
                activityType: ActivityType.Research
            }
            this.SendToServer(ClientResponseType.SendWaifuToActivity, JSON.stringify(u), this.user.Id)
        },
        cancelResearch()
        {
            this.SendToServer(ClientResponseType.CancelActivity, this.activity!.id.toString(), this.user.Id)
        }
    },
    components: {

    },
    mounted() {
        setInterval(() => this.date_milli = Date.now(), 1000)
        //This is necessary for the value of date_milli to be updated so the computed value can also be updated
    },
    computed: {
        activity() {
            return this.user.activities.find(activity => activity.researchID == this.researchNode.id)
        }
    },
    emits: ["show-waifu-selector", "reset-selected-waifu"],
}


</script>
<template>
    <div class="researchNodeWithWaifu">

        <div v-if="selectedWaifu != null || activity != null">
            <div class="waifuSlot" @click="$emit('show-waifu-selector')">
                <img
                    :src="`${publicPath}waifu-image/${(selectedWaifu?.imgPATH || user.waifus[activity!.waifuID].imgPATH)}`">
            </div>
            <button v-if="activity == null" class="smallbutton nnnbutton" id="sendbutton"
                @click="sendWaifuToResearch()">{{$t("activities.start")}}</button>
            <button v-else-if="! activity.finished" class="smallbutton nnnbutton" id="sendbutton"  @click="cancelResearch()">{{$t("activities.cancel")}}</button>
            <button v-else class="smallbutton nnnbutton" id="sendbutton" @click="getActivityClaim()">{{$t("activities.claim")}}</button>
        </div>

        <div :style="getBackgroundColor()" class="researchNode" @click="$emit('show-waifu-selector')">

            <div data-no-dragscroll style="text-align: center;">
                {{ $t(`activities.research.${researchNode.id}`) }}
            </div>
            <div data-no-dragscroll id="time-cost">
                <span>{{ researchNode.cost }}</span>
                <img style="filter:brightness(0); margin-left: 2px;" height=24px width=24px src="@/assets/clock.svg">
            </div>
            <div v-if="activity != null && !activity.finished" id="activityContainer">
                <div id="activityBorder">
                    <div id="timeleft" :style="getTimeLeft(activity)">
                        <span> {{ getTimeLeftNumber(activity) }}</span>
                    </div>
                    <div id="bar-background"></div>
                </div>
            </div>
        </div>
    </div>

</template>


<style lang="css" scoped>

#time-cost
{
    margin-left: 10px;
    display: flex;
    flex-direction: row;
    place-items: center;
}

.researchNode {
    cursor: auto;
    width: 330px;
    height: 60px;
    color: black;
    padding: 10px;
    border-radius: 10px;
    /*border: 5px;
    border-style: solid;
    border-color:gray;*/
}

.researchNodeWithWaifu {
    display: flex;
}

.waifuSlot {
    position: absolute;
    left: -90px;
    margin : 0px;
    margin-right: 10px;
    border-radius: 10px;
    border-width: 3px;
    width: 74px;
    height: 74px;
    overflow: hidden;
    border-color:purple;
}

.waifuSlot img {
    width: 74px;
}

#sendbutton {
    position: absolute;
    height:35px;     /*since it's a techno tree with a draggable field, pixel values shouldn't matter too much... right?*/
    padding:0px 8px;
    bottom: -42px;
    left: -87px;
}

#activityContainer {
    display: flex;
    justify-content: center;
    margin-bottom: 20px;
}

#activityBorder {
    position: absolute;
    height: 30px;
    top:75px;
    left:0;
    width: 340px;
    border-radius: 30px;
    display: flex;
    align-items: center;
    border: 4px;
    border-style: solid;
    border-color:rgb(22, 0, 144);
}

#activityBorder span {
    position: absolute;
    left: 50%;
    
    transform: translateX(-50%);
    font-size: larger;
}

#timeleft {
    z-index: 2;
    height: 30px;
    justify-self: end;
    border-radius: 30px;
    background-color: rgb(43, 154, 191);
}

#bar-background {
    position: absolute;
    width:340px;
    height: 30px;
    z-index: 1;
    justify-self: end;
    border-radius: 30px;
    background-color: rgb(168, 233, 255);
}
</style>