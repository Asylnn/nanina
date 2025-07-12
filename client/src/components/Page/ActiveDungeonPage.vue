<script lang="ts">
import ActiveDungeon from '@/classes/dungeons/active_dungeon';
import User from '@/classes/user/user';
import config from '../../../../baseValues.json'
import { WebsocketEvent, type Websocket } from 'websocket-ts';
import type WebSocketReponse from '@/classes/web_socket_response'

export default {
    name : "ActiveDungeonPage",
    data() {
        return {
            publicPath : import.meta.env.BASE_URL,
            date_milli: Date.now(),
        }

    },
    props: {
        active_dungeon: {
            type : ActiveDungeon,
            required : true
        },
        user : {
            type : User,
            required: true
        }
    },
    emits : ["leave-dungeon"],
    methods:{
        LeaveDungeon(){
            this.$emit("leave-dungeon")
            this.SendToServer("stop dungeon", this.active_dungeon.instanceId, this.user.Id)
        },
        getHealthBarStyle()
        {
            return " width:" + 60*(this.active_dungeon.health/this.active_dungeon.maxHealth) + "vw";
        },
        claimDungeon()
        {
            this.user.claimTimestamp = Date.now()
            User.updateTimer(this.user)
            this.SendToServer("claim dungeon fight", this.active_dungeon.instanceId, this.user.Id)
            
        }
    },
    mounted(){
        
        setInterval(() => this.date_milli = Date.now(), 1000)
        //This is necessary for the value of date_milli to be updated so the computed value can also be updated
    },
    computed:
    {
        mapURL(){
            return `${this.active_dungeon.beatmap.url}#${this.active_dungeon.beatmap.mode}/${this.active_dungeon.beatmap.id}`
        },
        claimWaitTime()
        {
            return Math.ceil((this.user.claimTimestamp + config.time_for_allowing_another_claim_in_milliseconds - this.date_milli)/60000*60)
        },
    },
}

</script>

<template>
    <div >
        <div>
            <h1>{{$t(`dungeon.${active_dungeon.template.id}.name`)}} </h1>
            <div id="healthBarContainer">
                <div id="healthBarBorder">
                    <div id="healthBar" :style="getHealthBarStyle()">
                    </div>
                    <span>{{ Math.floor(active_dungeon.health) }}/{{ Math.floor(active_dungeon.maxHealth) }}</span>
                </div>
            </div>
            <div class="flex margins">
                <span>
                    {{ $t("dungeon.challenge") }}
                </span>
                <span style="align-self: center;">
                    <a :href="mapURL">{{ `${active_dungeon.beatmap.beatmapset.artist} - ${active_dungeon.beatmap.beatmapset.title} [${active_dungeon.beatmap.version}] (${active_dungeon.beatmap.beatmapset.creator}, ${active_dungeon.beatmap.difficulty_rating}★)` }}</a>
                </span>
                <!---->
                <div class="buttonHolder">
                    <button class="smallbutton nnnbutton" @click="claimDungeon">{{ $t("fight.download") }}</button>
                    <button class="smallbutton nnnbutton" @click="claimDungeon">{{ $t("dungeon.fight") }}</button>
                </div>
                <div class="waifuInfo">
                    <div class="waifuSlot">
                        <img :src="`${publicPath}waifu-image/${active_dungeon.waifus[0].imgPATH}`">
                    </div>
                    <div class="log">
                        <div v-for="log in active_dungeon.log.slice(-30).filter(log => log.waifuId == active_dungeon.waifus[0].id ).reverse()">
                            <p class="attackLine">{{ $t("dungeon.attack", {waifu_name:$t(`waifu.${log.waifuId}.name`), attack_type:log.attackType, damage:Math.floor(log.dmg)}) }}</p>
                        </div>
                    </div>
                </div>
                <div class="waifuInfo">
                    <div class="waifuSlot">
                        <img :src="`${publicPath}waifu-image/${active_dungeon.waifus[1].imgPATH}`">
                    </div>
                    <div class="log">
                        <div v-for="log in active_dungeon.log.slice(-30).filter(log => log.waifuId == active_dungeon.waifus[1].id ).reverse()">
                            <p class="attackLine">{{ $t("dungeon.attack", {waifu_name:$t(`waifu.${log.waifuId}.name`), attack_type:log.attackType, damage:Math.floor(log.dmg)}) }}</p>
                        </div>
                    </div>
                </div>
                <div class="waifuInfo">
                    <div class="waifuSlot">
                        <img :src="`${publicPath}waifu-image/${active_dungeon.waifus[2].imgPATH}`">
                    </div>
                    <div class="log">
                        <div v-for="log in active_dungeon.log.slice(-30).filter(log => log.waifuId == active_dungeon.waifus[2].id ).reverse()">
                            <p class="attackLine">{{ $t("dungeon.attack", {waifu_name:$t(`waifu.${log.waifuId}.name`), attack_type:log.attackType, damage:Math.floor(log.dmg)}) }}</p>
                        </div>
                    </div>
                </div>
                <div class="waifuInfo boss">
                    
                    
                    <div class="waifuSlot">
                        <img :src="`${publicPath}waifu-image/unknown.svg`">
                    </div>
                    <!--<div class="log">
                        <div v-for="log in active_dungeon.log.slice(-30).filter(log => log.waifuId == active_dungeon.waifus[0].id ).reverse()">
                            <p class="attackLine">{{ $t("dungeon.attack", {waifu_name:$t(`waifu.${log.waifuId}.name`), attack_type:log.attackType, damage:Math.floor(log.dmg)}) }}</p>
                        </div>
                    </div>-->
                </div>
                <button class="leavebutton smallbutton nnnbutton" @click="LeaveDungeon">{{ $t("dungeon.leave") }}</button>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

#waifuSelection
{
    display: flex;
}

h1
{
    text-align: center;
    margin-bottom: 20px;
}

.waifuInfo
{
    margin: 25px 0px;
    height: 15vw;
    display:flex;
}
.waifuInfo .log
{
    margin-top: 20px;
}

.boss
{
   flex-direction: row-reverse;
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

#attackLines {

    height: 10vh;
    width: 35vw;
    font-size: large;
    color:bisque;
    overflow: scroll;
}
#healthBarContainer
{
    display:flex;
    height: 60px;
    justify-content: center;
    margin-bottom: 20px;
}

#healthBarBorder
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

#healthBarBorder span
{
    position:absolute;
    left:50%;
    transform: translateX(-50%);
    font-size: larger;
}

#healthBar
{
    height: 40px;
   
    justify-self: end;
    border-radius: 100px;
    border-style: none;
    border-width: 10px;
    background-color: red;
}

#bgMap
{
    max-width: 80vw;
}



.flex span
{
    margin-bottom: 15px;
}

.buttonHolder
{
    display: grid;
    justify-content: center;
    grid-template-columns: 1fr 1fr; 
}

.buttonHolder button
{
    margin: 0px 100px;
}

.leavebutton
{
    margin: 50px 0px;
    width: 300px;
    place-self: center;
}

/*#playingField {
    height: 50vh;
    width: 100vw;
    background-image: url("src/assets/playingField.jpg");
    background-size: contain;
    background-repeat: no-repeat;
}*/


</style>