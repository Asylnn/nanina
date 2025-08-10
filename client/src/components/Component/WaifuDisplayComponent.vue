<script lang="ts">
import Waifu from '@/classes/waifu/waifu';
import GridDisplayComponent from './GridDisplayComponent.vue';
import WaifuStatDisplayComponent from './WaifuStatDisplayComponent.vue';

import User from '@/classes/user/user';
import EquipmentPiece from '@/classes/item/piece';
import Equipment from '@/classes/item/equipment';
import ItemComponent from './ItemComponent.vue';
import StatModifier from '@/classes/modifiers/stat_modifier';
import ClientResponseType from '@/classes/client_response_type';

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
            let equip = true;
            if(this.selected_item == null) return;
            switch(this.equipmentPiece){
                case EquipmentPiece.Weapon:
                    equip = this.waifu.equipment.weapon?.inventoryId != this.selected_item?.inventoryId
                    this.waifu.equipment.weapon = this.selected_item
                    break;
                case EquipmentPiece.Dress:
                    equip = this.waifu.equipment.dress?.inventoryId != this.selected_item?.inventoryId
                    this.waifu.equipment.dress = this.selected_item
                    break;
                case EquipmentPiece.Accessory:
                    equip = this.waifu.equipment.accessory?.inventoryId != this.selected_item?.inventoryId
                    this.waifu.equipment.accessory = this.selected_item
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
            let oldEquipment = null as Equipment | null
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
            }
            this.SendToServer(ClientResponseType.UnequipItem, JSON.stringify({equipmentPiece:piece, waifuId:this.waifu.id}), this.user.Id)
        }
    },
    components:{
        GridDisplayComponent,
        ItemComponent,
        WaifuStatDisplayComponent,
    },
    emits:["click"],
    computed : {
        equipment_to_show() {
            return this.user.inventory.equipment.filter(equipment => equipment.piece == this.equipmentPiece)
        }
    }
}

</script>

<template>
    <div v-if="!forDungeon && !forPull" class="equipment">
        <div v-if="inventoryVisible" @click="inventoryVisible = false" class="veil" id="inventoryveil"></div>
        <GridDisplayComponent v-if="inventoryVisible" id="grid" @show-element="openWeaponDisplay" :elements="equipment_to_show" :columns=5 :show-border="true"></GridDisplayComponent>
        <div v-if="weaponVisible" @click="closeWeaponDisplay" class="veil" id="itemveil"></div>
        <ItemComponent :userID="user.Id" @click="selectItem()" :is-for-equiping="true" v-if="selected_item != null" @input="" :item="selected_item"></ItemComponent>
    </div>

    <div id="focusedWaifu" @click="$emit('click')">
        <div id="waifuImage"><img :src="`${publicPath}/waifu-image/${waifu.imgPATH}`"></div>
        <div id="waifuInfos">
            <div class="shortStat">
                <span>{{ $t(`waifu.${waifu.id}.name`) }}</span> <span>{{ $t("waifu.level") }} {{ waifu.lvl }}</span>  <br>
                </div>
            
            
            <div v-if="!forPull">
                
                <div class="shortStat">
                    <span>XP</span> <span>{{ waifu.xp }}/{{ waifu.XpToLvlUp }} </span>   <br>
                </div><br>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.str" :statAmount="waifu.Str" :statModifier="StatModifier.STR"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.kaw" :statAmount="waifu.Kaw" :statModifier="StatModifier.KAW"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.int" :statAmount="waifu.Int" :statModifier="StatModifier.INT"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.agi" :statAmount="waifu.Agi" :statModifier="StatModifier.AGI"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.dex" :statAmount="waifu.Dex" :statModifier="StatModifier.DEX"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.luck" :statAmount="waifu.Luck" :statModifier="StatModifier.LUCK"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.physical" :statAmount="waifu.Physical" :statModifier="StatModifier.Physical"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.psychic" :statAmount="waifu.Psychic" :statModifier="StatModifier.Psychic"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.magical" :statAmount="waifu.Magical" :statModifier="StatModifier.Magical"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.crit_chance" :statAmount="waifu.CritChance" :statModifier="StatModifier.CritChance"></WaifuStatDisplayComponent>
                <WaifuStatDisplayComponent :waifu="waifu" statName="waifu.stats.crit_damage" :statAmount="waifu.CritDamage" :statModifier="StatModifier.CritDamage"></WaifuStatDisplayComponent>
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
    </div>
</template>

<style lang="css" scoped>
.shortStat {
    display: grid;
    grid-template-columns: 1fr 1fr;
}

.modifier {
    width:60px;
}

#waifuIcons {
    padding:0px;
    margin:10vh 20vw ;
    position:fixed;
    height: 80vh;
    overflow: scroll;
}

.equipment {
    display: flex;
}

#inventoryveil{
    z-index: 728;
}

#itemveil{
    z-index: 810;
}

#grid {
    z-index: 780;
    position: sticky;
    top : 120px;
    right: 0px;
    left : 0px;
    padding:0px;
    margin:10vh 20vw ;
    position:fixed;
    height: 80vh;
    overflow: scroll;
}

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

#focusedWaifu {
    position: fixed;
    width: 40vw;
    height: 55vh;
    border-radius: 15px;
    display: grid;
    grid-template-columns: 1fr 1fr;
    padding: 2vh 2vh;
    z-index: 727;
    top:50%;                                /*Make the display be at the center of the screen*/
    left:50%;
    transform: translate(-50%, -50%);
    background-color: rgb(6, 16, 26);
}

#focusedObject {
    z-index: 820;
}

#focusedWaifu img {
    max-width: 25vw;
    max-height: 60vh;
}

#waifuImage {
    overflow: hidden;
}

#waifuInfos {
    padding: 0 1vw;
}

</style>