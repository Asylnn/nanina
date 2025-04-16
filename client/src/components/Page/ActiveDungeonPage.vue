<script lang="ts">
import ActiveDungeon from '@/classes/dungeons/active_dungeon';
import User from '@/classes/user/user';

export default {
    name : "ActiveDungeonPage",
    data() {
        return {
            publicPath : import.meta.env.BASE_URL,
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
    },
    computed:
    {
        mapURL(){
            return `${this.active_dungeon.beatmap.url}#${this.active_dungeon.beatmap.mode}/${this.active_dungeon.beatmap.id}`
        }
    },
}

</script>

<template>
    <div >
        <div>
            <h1>{{$t(`dungeon.${active_dungeon.template.id}.name`)}} </h1>
            <div id="healthBarBox">
                <div id="healthBar" :style="getHealthBarStyle()"></div>
             </div>
            <button class="smallbutton" @click="LeaveDungeon">{{ $t("dungeon.leave") }}</button><br>
            <span>
                {{ $t("dungeon.challenge") }}
            </span><br>
            <a :href="mapURL"> <img id="bgMap" :src="active_dungeon.beatmap.beatmapset.covers.slimcover2x"></a>
            <button class="smallbutton" @click="LeaveDungeon">{{ $t("dungeon.fight") }}</button><br>
            <div id="waifuSelection">
                <div class="waifuSlot">
                    <img :src="`${publicPath}waifu-image/${active_dungeon.waifus[0].imgPATH}`">
                </div>
                <div class="waifuSlot">
                    <img :src="`${publicPath}waifu-image/${active_dungeon.waifus[1].imgPATH}`">
                </div>
                <div class="waifuSlot">
                    <img :src="`${publicPath}waifu-image/${active_dungeon.waifus[2].imgPATH}`">
                </div>
            </div>
            <div id="playingField">
                <p> {{$t("dungeon.boss_health")}} : {{ active_dungeon.health }}/{{ Math.floor(active_dungeon.maxHealth) }}</p>
                <div id="attackLines">
                    Attacks : 
                    <div v-for="log in active_dungeon.log">
                        <p class="attackLine">{{ $t("dungeon.attack", {waifu_name:$t(`waifu.${log.waifuId}.name`), attack_type:log.attackType, damage:Math.floor(log.dmg)}) }}</p>
                    </div>
                </div>
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

#playingField {
    height: 50vh;
    width: 100vw;
    background-image: url("src/assets/playingField.jpg");
    background-size: contain;
    background-repeat: no-repeat;
}

#attackLines {
    margin-top:37vh;
    margin-left: 5vw;
    height: 10vh;
    width: 35vw;
    font-size: small;
    color:bisque;
    overflow: scroll;
}

#healthBarBox
{
    height: 40px;
    width: 60vw;
    border-radius: 100px;
    border-style: solid;
    border-color: blueviolet;
    border-width: 10px;
}

#healthBar
{
    height: 40px;
   

    border-radius: 100px;
    border-style: none;
    border-width: 10px;
    background-color: red;
}

</style>