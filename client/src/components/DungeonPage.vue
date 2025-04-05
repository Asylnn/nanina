<script lang="ts">
import ActiveDungeon from '@/classes/dungeons/active_dungeon';
import DungeonTemplate from '@/classes/dungeons/template_dungeons';
import User from '@/classes/user/user';
import type Waifu from '@/classes/waifu/waifu';
import WaifuDisplayComponent from './WaifuDisplayComponent.vue';
import GridDisplayComponent from './GridDisplayComponent.vue';

export default {
    name : "DungeonPage",
    data() {
        return {
            selected_dungeon : null as string | null,
            is_fighting_a_dungeon : false,
            waifuSelectorVisible : false,
            waifuVisible : false,
            publicPath : import.meta.env.BASE_URL,
            waifuSelection : [null, null, null] as Array<Waifu | null>,
            availableWaifus : [] as Waifu[],
            selectedWaifu : null as Waifu | null,
            select: 0,
            selected_floor : null as number | null
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
    mounted(){
        this.availableWaifus = this.user.waifus
    },
    methods:{
        EnterDungeon(){
            this.is_fighting_a_dungeon = true
            let waifuIds = this.waifuSelection.map(waifu => waifu!.id)
            this.SendToServer("start dungeon", JSON.stringify({id:this.selected_dungeon, waifuIds:waifuIds, floor:this.selected_floor}), this.user.Id)
        },
        LeaveDungeon(){
            this.is_fighting_a_dungeon = false
            this.SendToServer("stop dungeon", this.active_dungeon.instanceId, this.user.Id)
        },
        unequip(selectNumber : number, e : Event)
        {
            e.preventDefault()
        },
        openWaifuSelectorDisplay(selectNumber : number)
        {
            this.select = selectNumber
            this.waifuSelectorVisible = true
        },
        openWaifuDisplay(waifu : Waifu)
        {
            this.selectedWaifu = waifu
            this.waifuVisible = true
        },
        closeWaifuDisplay()
        {
            this.waifuVisible = false
            this.selectedWaifu = null
        },
        selectWaifu()
        {
            if(this.waifuSelection[this.select] != null)
                this.availableWaifus.push(this.waifuSelection[this.select]!)
            this.waifuSelection[this.select] = this.selectedWaifu
            this.availableWaifus = this.availableWaifus.filter(waifu => waifu.id != this.selectedWaifu!.id)
            this.closeWaifuDisplay()
            this.waifuSelectorVisible = false
        },
        getfloorListClass(i: number)
        {
            if(i == this.selected_floor)
                return "selected"
        },
        getDungeonListClass(id:string)
        {
            if(id == this.selected_dungeon)
                return "selected"
        },
        validSelection()
        {
            return this.selected_dungeon != null && this.selected_floor != null && this.waifuSelection.every(waifu => waifu != null)  
        }
    },
    computed:
    {
        
    },
    components :{
        WaifuDisplayComponent,
        GridDisplayComponent
    }
}

</script>

<template>
    <div >
        <div v-if="waifuSelectorVisible">
            <div @click="waifuSelectorVisible = false" class="veil" id="waifuSelectorVeil"></div>
            <GridDisplayComponent id="grid" @show-element="openWaifuDisplay" :elements="availableWaifus" :columns=5></GridDisplayComponent>
        </div>
        
        <div v-if="waifuVisible" @click="closeWaifuDisplay" class="veil" id="waifuveil"></div>
        <WaifuDisplayComponent v-if="selectedWaifu != null" @click="selectWaifu" :for-pull="false" :for-dungeon="true"  :waifu="selectedWaifu" :user="user"></WaifuDisplayComponent>

        
        
        <div v-if="!is_fighting_a_dungeon" id="dungeonSelectionDisplay">
            <!--<div v-if="selectedValidWaifus">
                <select v-for="dungeon in dungeons" v-model="selected_dungeon">
                    <option :value="dungeon" >{{$t(`dungeon.${dungeon.id}.name`)}}</option>
                </select>
                <button @click="EnterDungeon()">Enter Dungeon</button>
            </div>-->
            
            <div id="waifuSelection">
                <div @click.right="unequip(0, $event)" @click="openWaifuSelectorDisplay(0)" class="waifuSlot clickable">
                    <img  :src="`${publicPath}waifu-image/${waifuSelection[0]?.imgPATH ?? 'unknown.svg'}`">
                </div>
                <div @click.right="unequip(1, $event)" @click="openWaifuSelectorDisplay(1)" class="waifuSlot clickable">
                    <img :src="`${publicPath}waifu-image/${waifuSelection[1]?.imgPATH ?? 'unknown.svg'}`">
                </div>
                <div @click.right="unequip(2, $event)" @click="openWaifuSelectorDisplay(2)" class="waifuSlot clickable">
                    <img :src="`${publicPath}waifu-image/${waifuSelection[2]?.imgPATH ?? 'unknown.svg'}`">
                </div>
            </div>
            <div>{{$t(`dungeon.floor`)}}</div>
            <div>
                <ul id="floorList">
                    <li v-for="i in [1,2,3,4,5]" :class="getfloorListClass(i)" class="clickable" @click="selected_floor = i">{{i}}</li>
                </ul>
            </div>
            <div style="margin-top: 30px;">{{$t(`dungeon.name`)}}</div>
            <div>
                <ul id="dungeonsList" >
                    <li v-for="dungeon in dungeons" :class="getDungeonListClass(dungeon.id)" class="clickable" style="margin-top: 10px;" @click="selected_dungeon = dungeon.id">{{$t(`dungeon.${dungeon.id}.name`)}}</li>
                </ul>
            </div>
            <div v-if="validSelection()" style="margin-top: 50px;">
                <button id="enterDungeonButton" @click="EnterDungeon()">Enter Dungeon</button>
            </div>
        </div>
        <div v-else>
            <button @click="LeaveDungeon">{{ $t("dungeon.leave") }}</button>
            <div id="playingField">
                <p> {{$t("dungeon.boss_health")}} : {{ active_dungeon.health }}/{{ Math.floor(active_dungeon.dungeonTemplate.maxHealth) }}</p>
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

#dungeonSelectionDisplay
{
    font-size:larger;
    display:flex;
    flex-direction: column;
    align-items: center;
    align-content: center;
    justify-content: center;
    justify-items: center;
}

#selectedWaifu
{
    cursor: pointer
}

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

#waifuSelectorVeil{
    z-index: 100;
}

#waifuveil{
    z-index: 200;
}

.grid {
    z-index: 780;
    position: sticky;
    top : 10px;
}

#waifuSelection
{
    display: flex;
}

#floorSelect
{
}

#floorList
{
    display:flex;
}

#floorList li
{
    margin-right: 15px;
}

#dungeonsList
{
    margin-top: 15px;
    display: flex;
    flex-direction: column;
    align-items: center;
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

.clickable
{
    cursor: pointer;
    color: rgb(203, 165, 221);
}

.clickable:hover
{
    color:purple;
}

.selected
{
    color:rgb(178, 77, 224);
    text-decoration:underline;
}

#enterDungeonButton
{
    padding:10px 20px;
    border-color:rgb(178, 77, 224);
    border-width: 4px;
    border-style: solid;
    border-radius: 40px;
    
}

#enterDungeonButton:hover
{
    color:purple;
}
</style>