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
        },
        getTimeLeftNumber(activity: Activity) {
            return MillisecondsToHourMinuteSecondFormat(activity.timestamp + activity.timeout - this.date_milli)
        },
        getTimeLeft(activity: Activity) {
            return " width:" + 330 * (activity.timestamp + activity.timeout - this.date_milli) / activity.originalTimeout + "px";
        },
        getBackgroundColor() {
            var style = "background-color:"
            if(this.activity != null)
            {
                style += "lightblue;"
                style += "cursor: pointer;"
            }
            if (this.user.completedResearches.some(research => research.id == this.researchNode.id) && !this.researchNode.infinite) {
                style += "green;"
            }

            else if (this.researchNode.requirements.every(researchID => this.user.completedResearches.some(research => research.id == researchID))) {
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
    emits: ["show-waifu-selector"],
}


</script>
<template>
    <div class="researchNodeWithWaifu">

        <div v-if="selectedWaifu != null || activity != null">
            <div class="waifuSlot" @click="$emit('show-waifu-selector')">
                <img
                    :src="`${publicPath}waifu-image/${(selectedWaifu || user.waifus.find(waifu => waifu.id == activity!.waifuID))!.imgPATH}`">
            </div>
            <button v-if="activity == null" class="smallbutton nnnbutton" id="sendbutton"
                @click="sendWaifuToResearch()">send waifu</button>
            <button v-else-if="! activity.finished" class="smallbutton nnnbutton" id="sendbutton">cancel</button>
            <button v-else class="smallbutton nnnbutton" id="sendbutton" @click="getActivityClaim()">finish</button>
        </div>

        <div :style="getBackgroundColor()" class="researchNode" @click="$emit('show-waifu-selector')">

            <div data-no-dragscroll>
                {{ $t(`activity.research.${researchNode.id}`) }}
            </div>
            <div data-no-dragscroll>
                {{ researchNode.cost }}
            </div>
            <div v-if="activity != null && !activity.finished" id="activityContainer">
                <div id="activityBorder">
                    <div id="timeleft" :style="getTimeLeft(activity)">
                    </div>
                    <span> {{ getTimeLeftNumber(activity) }}</span>
                </div>
            </div>
        </div>
    </div>

</template>


<style lang="css" scoped>
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
    margin-right: 10px;
    /*border: 10px;
    border-style: solid;
    border-color:rgb(20,20,20);*/
    border-radius: 10px;
    width: 80px;
    height: 80px;
    overflow: hidden;
}

.waifuSlot img {
    width: 80px;
}

#sendbutton {
    position: absolute;
    bottom: -50px;
    right: 50px;
}




#activityContainer {
    display: flex;
    justify-content: center;
    margin-bottom: 20px;
}

#activityBorder {
    width: 330px;
    border-radius: 10px;
    display: flex;
    align-items: center;
    border: 2px;
    border-style: solid;
    border-color:rgb(20,20,20);
}

#activityBorder span {
    position: absolute;
    left: 50%;
    transform: translateX(-50%);
    font-size: larger;
}

#timeleft {
    height: 25px;
    justify-self: end;
    border-radius: 7px;
    background-color: purple;
}
</style>