<script lang="ts">
import Waifu from '@/classes/waifu/waifu';
import GridDisplayComponent from './GridDisplayComponent.vue';
import WaifuStatDisplayComponent from './WaifuStatDisplayComponent.vue';
import sets from '../../../../save/set.json'
import Set from '@/classes/item/set';
import User from '@/classes/user/user';
import EquipmentPiece from '@/classes/item/piece';
import Equipment from '@/classes/item/equipment';
import ItemComponent from './ItemComponent.vue';
import StatModifier from '@/classes/modifiers/stat_modifier';
import ClientResponseType from '@/classes/client_response_type';
import ModifierComponent from './ModifierComponent.vue';
import DisplayComponent from './DisplayComponent.vue';
import { WebsocketEvent, type Websocket } from 'websocket-ts';
import type WebSocketResponse from '@/classes/web_socket_response';
import ServerResponseType from '@/classes/server_response_type';
import type Dictionary from '@/classes/dictionary';

export default {
    name : "WaifuDisplayComponent",
    data() {
        return {
            publicPath : import.meta.env.BASE_URL,
            EquipmentPiece: EquipmentPiece,
            StatModifier: StatModifier,
            equipmentPiece: EquipmentPiece.Weapon,
            //equipment_to_show : [] as Array<Equipment>, 
            inventoryVisible: false,
            listener: (() => {}) as (i: Websocket, ev: MessageEvent) => void,
            weaponVisible: false,
            selected_item: null as Equipment | null,
        }
    },
    props: {
        forPull: {
            type : Boolean,
            required : true,
        },
        forDungeon: {
            type : Boolean,
            required : true,
        },
        waifu: {
            type : Waifu,
            required : true
        },
        count: { //undefined for single pull in gacha or for a display in WaifuListPage
            type : Number,
            required : false
        },
        user: {
            type : User,
            required : true
        }
    },
    methods:{

        openWeaponDisplay(equipment : Equipment){
            this.selected_item = equipment
            this.weaponVisible = true
        },
        closeWeaponDisplay()
        {
            this.selected_item = null
            this.weaponVisible = false
        },
        closeAllDisplays()
        {
            this.closeWeaponDisplay()
            this.inventoryVisible = false
        },
        selectItem()
        {
            let equip = true; //Not sure testing this in client is necessary...
            if(this.selected_item == null) return;
            switch(this.equipmentPiece){
                case EquipmentPiece.Weapon:
                    equip = this.waifu.equipment.weapon?.inventoryId != this.selected_item?.inventoryId
                    //this.waifu.equipment.weapon = this.selected_item
                    break;
                case EquipmentPiece.Dress:
                    equip = this.waifu.equipment.dress?.inventoryId != this.selected_item?.inventoryId
                    //this.waifu.equipment.dress = this.selected_item
                    break;
                case EquipmentPiece.Accessory:
                    equip = this.waifu.equipment.accessory?.inventoryId != this.selected_item?.inventoryId
                    //this.waifu.equipment.accessory = this.selected_item
                    break;
            }
            
            console.log('equiping')
            if(equip)
                this.SendToServer(ClientResponseType.EquipItem, JSON.stringify({equipmentId:this.selected_item.inventoryId, waifuId:this.waifu.id}), this.user.Id)
            this.closeAllDisplays()
        },
        openDisplay(piece : EquipmentPiece)
        {
            this.inventoryVisible = true; 
            this.equipmentPiece = piece
            switch(piece){
                case EquipmentPiece.Weapon:
                    if(this.waifu.equipment.weapon != null)
                    {
                        this.weaponVisible = true
                        this.selected_item = this.waifu.equipment.weapon
                    }
                    break;
                case EquipmentPiece.Dress:
                    if(this.waifu.equipment.dress != null)
                        {
                            this.weaponVisible = true
                            this.selected_item = this.waifu.equipment.dress
                        }
                    break;
                case EquipmentPiece.Accessory:
                    if(this.waifu.equipment.accessory != null)
                        {
                            this.weaponVisible = true
                            this.selected_item = this.waifu.equipment.accessory
                        }
                    break;
            }
        },
        unequip(piece: EquipmentPiece, e : Event)
        {
            e.preventDefault()
            /*let oldEquipment = null as Equipment | null
            switch(piece){
                case EquipmentPiece.Weapon:
                    this.waifu.equipment.weapon = null
                    break;
                case EquipmentPiece.Dress:
                    this.waifu.equipment.dress = null
                    break;
                case EquipmentPiece.Accessory:
                    this.waifu.equipment.accessory = null
                    
                    break;
            }*/
            this.SendToServer(ClientResponseType.UnequipItem, JSON.stringify({equipmentPiece:piece, waifuId:this.waifu.id}), this.user.Id)
        }
    },
    components:{
        GridDisplayComponent,
        ItemComponent,
        WaifuStatDisplayComponent,
        ModifierComponent,
        DisplayComponent,
    },
    emits:["click", "exit"],
    computed : {
        equipment_to_show() {
            return Object.values(this.user.inventory.equipment).filter(equipment => equipment.piece == this.equipmentPiece)
        }
    },
    mounted()
    {
        this.listener = (i: Websocket, ev: MessageEvent) => {
			var res : WebSocketResponse = JSON.parse(ev.data)
			switch (res.type) 
            {
                case ServerResponseType.ConfirmEquip:
                    let InventoryId : number = res.data
                    let equipment = this.user.inventory.equipment[InventoryId]
                    delete this.user.inventory.equipment[InventoryId]
                    let oldEquipment : Equipment | null
                    switch(equipment.piece)
                    {
                        case EquipmentPiece.Dress:
                            oldEquipment = this.waifu.equipment.dress
                            this.waifu.equipment.dress = equipment
                            break;
                        case EquipmentPiece.Weapon:
                            oldEquipment = this.waifu.equipment.weapon
                            this.waifu.equipment.weapon = equipment
                            break;
                        case EquipmentPiece.Accessory:
                            oldEquipment = this.waifu.equipment.accessory
                            this.waifu.equipment.accessory = equipment
                            break;
                    }
                    if(this.waifu.equipment.weapon?.setId == this.waifu.equipment.dress?.setId && this.waifu.equipment.dress?.setId == this.waifu.equipment.accessory?.setId && this.waifu.equipment.weapon?.setId != null)
                        this.waifu.equipment.set = (sets as Dictionary<Set>)[equipment.setId]
                    else
                        this.waifu.equipment.set = null;
                    if(oldEquipment != null)
                        this.user.inventory.AddEquipment(oldEquipment)
                    break;
                case ServerResponseType.ConfirmUnequip:
                    let piece : EquipmentPiece = +res.data
                    let unequipedEquipment : Equipment | null = null
                    console.log(piece)
                    console.log(+piece)
                    switch(piece)
                    {
                        case EquipmentPiece.Dress:
                            unequipedEquipment = this.waifu.equipment.dress
                            this.waifu.equipment.dress = null
                            console.log(this.waifu.equipment.dress)
                            break;
                        case EquipmentPiece.Weapon:
                            unequipedEquipment = this.waifu.equipment.weapon
                            this.waifu.equipment.weapon = null
                            break;
                        case EquipmentPiece.Accessory:
                            unequipedEquipment = this.waifu.equipment.accessory
                            this.waifu.equipment.accessory = null
                            break;
                    }
                    this.waifu.equipment.set = null;
                    console.log(this.waifu.equipment)
                    if(unequipedEquipment != null)
                        this.user.inventory.AddEquipment(unequipedEquipment)
                    break;
            }
        }
        //@ts-ignore
        this.ws.addEventListener(WebsocketEvent.message, this.listener);
    },
    unmounted()
    {
        //@ts-ignore
        this.ws.removeEventListener(WebsocketEvent.message, this.listener)
    },
}

</script>

<template>

    <div v-if="!forDungeon && !forPull" id="itemDisplay">
        <div v-if="inventoryVisible" @click="inventoryVisible = false" class="veil" id="inventoryveil"></div>
        <GridDisplayComponent 
            v-if="inventoryVisible" 
            id="grid" 
            @show-element="openWeaponDisplay" 
            :elements="equipment_to_show" 
            :columns=5 
            :show-border="true"
            :sticky-waifu-grid="true"
            :show-item="true">
        </GridDisplayComponent>
        <div v-if="weaponVisible" @click="closeWeaponDisplay" class="veil" id="itemveil"></div>
        <ItemComponent v-if="selected_item != null" :userID="user.Id" @exit="closeWeaponDisplay" @click="selectItem()" :is-for-equiping="true"  @input="" :item="selected_item"></ItemComponent>
    </div>

    <DisplayComponent @exit="$emit('exit')" :type="'waifu'">
        <div id="waifuImage" @click="$emit('click')"><img :src="`${publicPath}/waifu-image/${waifu.imgPATH}`"></div>
        <div id="waifuInfos" @click="$emit('click')">
            <div class="shortStat">
                <span>{{ $t(`waifu.${waifu.id}.name`) }}</span> <span>{{ $t("waifu.level") }} {{ waifu.lvl }}</span>  <br>
                </div>
            
            
            <div v-if="!forPull">
                
                <div class="shortStat">
                    <span>XP</span> <span>{{ waifu.xp }}/{{ waifu.XpToLvlUp }} </span>   <br>
                </div><br>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.str" :statAmount="waifu.str" :statModifier="StatModifier.STR"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.kaw" :statAmount="waifu.kaw" :statModifier="StatModifier.KAW"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.int" :statAmount="waifu.int" :statModifier="StatModifier.INT"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.agi" :statAmount="waifu.agi" :statModifier="StatModifier.AGI"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.dex" :statAmount="waifu.dex" :statModifier="StatModifier.DEX"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.luck" :statAmount="waifu.luck" :statModifier="StatModifier.LUCK"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.physical" :statAmount="waifu.physical" :statModifier="StatModifier.Physical"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.psychic" :statAmount="waifu.psychic" :statModifier="StatModifier.Psychic"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.magical" :statAmount="waifu.magical" :statModifier="StatModifier.Magical"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.crit_chance" :statAmount="waifu.critChance" :statModifier="StatModifier.CritChance"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.crit_damage" :statAmount="waifu.critDamage" :statModifier="StatModifier.CritDamage"></WaifuStatDisplayComponent>
                <div v-if="!forDungeon" class="equipment">
                    <div @click.right="unequip(EquipmentPiece.Weapon, $event)" @click="openDisplay(EquipmentPiece.Weapon)" class="itemSlot">
                        <img  :src="`${publicPath}item-image/${waifu.equipment.weapon?.imgPATH ?? 'unknown.svg'}`">
                    </div>
                    <div @click.right="unequip(EquipmentPiece.Dress, $event)" @click="openDisplay(EquipmentPiece.Dress)" class="itemSlot">
                        <img :src="`${publicPath}item-image/${waifu.equipment.dress?.imgPATH ?? 'unknown.svg'}`">
                    </div>
                    <div @click.right="unequip(EquipmentPiece.Accessory, $event)" @click="openDisplay(EquipmentPiece.Accessory)" class="itemSlot">
                        <img :src="`${publicPath}item-image/${waifu.equipment.accessory?.imgPATH ?? 'unknown.svg'}`">
                    </div>
                </div><br>
                <div v-if="waifu.equipment.set != null">
                    <span> {{ "Set Bonus" }}</span>
                    <div v-for="modifier in waifu.equipment.set.modifiers">
                        <ModifierComponent :modifier="modifier"></ModifierComponent>
                    </div>
                </div>
                
            </div>
            <div v-else>
                <span v-if="count != undefined">{{ $t("gacha.pull_number") }} {{ count+1 }}</span><br>
                {{ $t("waifu.stats.str") }} : {{ waifu.b_str }}<br>
                {{ $t("waifu.stats.kaw") }} : {{ waifu.b_kaw }}<br>
                {{ $t("waifu.stats.int") }} : {{ waifu.b_int }}<br>
                {{ $t("waifu.stats.agi") }} : {{ waifu.b_agi }}<br>
                {{ $t("waifu.stats.dex") }} : {{ waifu.b_dex }}<br>
                {{ $t("waifu.stats.luck") }} : {{ waifu.b_luck }}<br>
            </div>
        </div>
    </DisplayComponent>
</template>

<style lang="css" scoped>

.shortStat {
    display: grid;
    grid-template-columns: 1fr 1fr;
}

.modifier {
    width:60px;
}

.equipment {
    display: flex;
}

#inventoryveil{
    z-index: 125;
}

#itemveil{
    z-index: 175;
}

#display {
    max-width: 85vw;
    height: 550px;
}

#waifuImage img {
    max-width: 50vw;
    max-height: 60vh;
    left:10%;
    /*transform: translateX(50%);*/
}

@media only screen and (orientation: landscape) {
    #waifuImage img {
        max-width: 20vw;
        max-height: 80vh;
    }
}

#waifuImage {
    overflow: hidden;
}

#waifuInfos {
    padding: 0 1vw;
}

/*Equiping from inventory*/ 
/*
#grid {
    z-index: 150;
    position: sticky;
    top : 120px;
    right: 0px;
    left : 0px;
    padding:0px;
    margin:10vh 20vw ;
    position:fixed;
    height: 80vh;
    overflow: scroll;
}*/

.itemSlot{
    margin :10px;
    border: 10px;
    border-style: solid;
    border-radius: 20px;
    border-color:rgb(20,20,20);
    cursor: pointer;
}

.itemSlot img {
    width: 64px;
    height: 64px;
}

</style>