<script lang="ts">
import ActiveDungeon from '@/classes/dungeons/active_dungeon';
import DungeonTemplate from '@/classes/dungeons/template_dungeons';
import User from '@/classes/user/user';


export default {
    name : "DungeonPage",
    data() {
        return {
            selected_dungeon : this.dungeons[0],
            is_fighting_a_dungeon : false
        }

    },
    props: {
        dungeons: {
            type : Array<DungeonTemplate>,
            required : true
        },
        active_dungeon: { //= -1 for single pull in gacha or for a display in WaifuListPage
            type : ActiveDungeon,
            required : true
        },
        user : {
            type : User,
            required: true
        }
    },
    methods:{
        EnterDungeon(){
            this.is_fighting_a_dungeon = true
            this.SendToServer("start dungeon", this.selected_dungeon.id, this.user.Id)
        },
        LeaveDungeon(){
            this.is_fighting_a_dungeon = false
            this.SendToServer("stop dungeon", this.active_dungeon.instanceId, this.user.Id)
        }
    }
}

</script>

<template>
    <div>
        <div v-if="!is_fighting_a_dungeon">
            <select v-for="dungeon in dungeons" v-model="selected_dungeon">
                <option :value="dungeon" >{{$t(`dungeon.${dungeon.id}.name`)}}</option>
            </select>
            <button @click="EnterDungeon()">Enter Dungeon</button>
        </div>
        <div v-else>
            <button @click="LeaveDungeon">Leave Dungeon</button>
            <div id="playingField">
                <p>Boss Health : {{ active_dungeon.health }}/{{ Math.floor(active_dungeon.dungeonTemplate.maxHealth) }}</p>
                <div id="attackLines">
                    Attacks : 
                    <div v-for="log in active_dungeon.log">
                        <p class="attackLine">{{ log.waifuName }} with id {{ log.waifuId }} dealt {{ Math.floor(log.dmg) }} {{log.attackType}} damage to the boss!</p>
                    </div>
                </div>
            </div>
            <div v-if="active_dungeon.isCompleted">
                <p>Log : </p>
                {{ JSON.stringify(active_dungeon.loot) }}
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

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

</style>