<script lang="ts">
import DungeonTemplate from '@/classes/dungeons/template_dungeons';
import User from '@/classes/user/user';
import type Waifu from '@/classes/waifu/waifu';
import WaifuDisplayComponent from '../Component/WaifuDisplayComponent.vue';
import GridDisplayComponent from '../Component/GridDisplayComponent.vue';
import ClientResponseType from '@/classes/client_response_type';
import type Dictionary from '@/classes/dictionary';
import dungeonsJSON from  '@/../../save/dungeons.json'

export default {
    name : "DungeonSelectionPage",
    data() {
        return {
            dungeons : dungeonsJSON as Dictionary<DungeonTemplate>,
            selected_dungeon : null as string | null,
            is_fighting_a_dungeon : false,
            waifuSelectorVisible : false,
            waifuVisible : false,
            publicPath : import.meta.env.BASE_URL,
            waifuSelection : [null, null, null] as Array<Waifu | null>,
            availableWaifus : [] as Waifu[],
            selectedWaifu : null as Waifu | null,
            select: 0,
            selected_floor : null as number | null,
        }

    },
    props: {
        user : {
            type : User,
            required: true
        }
    },
    components :{
        WaifuDisplayComponent,
        GridDisplayComponent,
    },
    methods:{
        EnterDungeon(){
            this.is_fighting_a_dungeon = true
            let waifuIds = this.waifuSelection.map(waifu => waifu!.id)
            let obj = JSON.stringify({id:this.selected_dungeon, waifuIds:waifuIds, floor:this.selected_floor})
            this.SendToServer(ClientResponseType.StartDungeon, obj, this.user.Id)
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
            if(i == 3 || i == 4 || i == 5)
                    return "locked"
            else
                return "clickable"
        },
        onFloorClick(i: number)
        {
            if(i != 3 && i != 4 && i != 5)
                this.selected_floor = i
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
    mounted()
    {
        this.availableWaifus = this.user.availableWaifus
    }
}

</script>

<template>
    <div >
        <div v-if="waifuSelectorVisible">
            <div @click="waifuSelectorVisible = false" class="veil" id="waifuSelectorVeil"></div>
            <GridDisplayComponent id="grid" @show-element="openWaifuDisplay" :elements="availableWaifus" :columns=5 :sticky-waifu-grid="true"></GridDisplayComponent>
        </div>
        
        <div v-if="waifuVisible" @click="closeWaifuDisplay" class="veil" id="waifuveil"></div>
        <WaifuDisplayComponent v-if="selectedWaifu != null" @click="selectWaifu" @exit="closeWaifuDisplay" :for-pull="false" :for-dungeon="true"  :waifu="selectedWaifu" :user="user"></WaifuDisplayComponent>

        
        
        <div id="dungeonSelectionDisplay">
            
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
                    <li v-for="i in [1,2,3,4,5]" :class="getfloorListClass(i)" class="" @click="onFloorClick(i)">{{i}}</li>
                </ul>
            </div>
            <div style="margin-top: 30px;">{{$t(`dungeon.name`)}}</div>
            <div>
                <ul id="dungeonsList" >
                    <li v-for="dungeon in dungeons" :class="getDungeonListClass(dungeon.id)" class="clickable" style="margin-top: 10px;" @click="selected_dungeon = dungeon.id">{{$t(`dungeon.${dungeon.id}.name`)}}</li>
                </ul>
            </div>
            <div v-if="validSelection()" style="margin-top: 50px;">
                <button id="enterDungeonButton" class="nnnbutton" @click="EnterDungeon()">{{$t(`dungeon.enter_dungeon`)}}</button>
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

#waifuSelectorVeil{
    z-index: 30;
}

#waifuveil{
    z-index: 80;
}

#waifuSelection
{
    display: flex;
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

#enterDungeonButton
{
    padding:10px 20px;
}

#enterDungeonButton:hover
{
    border-color:rgb(203, 165, 221);
}

</style>