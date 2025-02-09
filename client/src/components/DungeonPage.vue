<script lang="ts">
import ActiveDungeon from '@/classes/dungeons/active_dungeon';
import DungeonTemplate from '@/classes/dungeons/template_dungeons';
import User from '@/classes/user';


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
            this.ws.send(JSON.stringify({type:"start dungeon", data:this.selected_dungeon.id, id: this.user.Id}))
        },
        LeaveDungeon(){
            this.is_fighting_a_dungeon = false
            this.ws.send(JSON.stringify({type:"stop dungeon", data:this.active_dungeon.instanceId, id: this.user.Id}))
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
            <p>Boss Health : {{ active_dungeon.health }}/{{ active_dungeon.dungeonTemplate.maxHealth }}</p>
            <p>Log : </p>
            <div v-if="active_dungeon.isCompleted">
                {{ JSON.stringify(active_dungeon.loot) }}
            </div>
            <div v-for="log in active_dungeon.log">
                <p>{{ log.waifuId }} dealt {{ Math.floor(log.dmg) }} {{log.attackType}} damage to the boss!</p>
            </div>
            
        </div>
    </div>
</template>

<style lang="css" scoped>

</style>