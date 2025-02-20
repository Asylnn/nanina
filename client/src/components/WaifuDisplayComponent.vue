<script lang="ts">
import Waifu from '@/classes/waifu/waifu';
import GridDisplayComponent from './GridDisplayComponent.vue';
import User from '@/classes/user/user';
import EquipmentPiece from '@/classes/item/piece';
import Equipment from '@/classes/item/equipment';
import ItemComponent from './ItemComponent/ItemComponent.vue';
import StatModifier from '@/classes/modifiers/stat_modifier';

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
                this.SendToServer("equip item", JSON.stringify({equipmentId:this.selected_item.inventoryId, waifuId:this.waifu.id}), this.user.Id)
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
            this.SendToServer("unequip item", JSON.stringify({equipmentPiece:piece, waifuId:this.waifu.id}), this.user.Id)
        }
    },
    components:{
        GridDisplayComponent,
        ItemComponent
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
        <GridDisplayComponent v-if="inventoryVisible" id="grid" @show-element="openWeaponDisplay" :elements="equipment_to_show" :columns=5></GridDisplayComponent>
        <div v-if="weaponVisible" @click="closeWeaponDisplay" class="veil" id="itemveil"></div>
        <ItemComponent @click="selectItem()" :is-for-equiping="true" v-if="selected_item != null" @input="" :item="selected_item"></ItemComponent>
    </div>

    <div id="focusedWaifu" @click="$emit('click')">
        <div id="waifuPic"><img :src="`${publicPath}/waifu-image/${waifu.imgPATH}`"></div>
        <div id="waifuInfos">
            <span>{{waifu.name}}</span>
            
            <div v-if="!forPull">
                
                <div class="stat">
                    <span>XP</span> <span> {{ waifu.xp }}/{{ waifu.XpToLvlUp }} </span>  <span>Level {{ waifu.lvl }}</span> <br>
                </div>
                
                <div class="stat">
                    <span>STR</span> <span>{{ Math.floor(waifu.Str) }}</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.STR)}})</span><br>
                </div>
                <div class="stat">
                    <span>KAW</span> <span>{{ Math.floor(waifu.Kaw) }}</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.KAW)}})</span><br>
                </div>
                <div class="stat">
                    <span>INT</span> <span>{{ Math.floor(waifu.Int) }}</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.INT)}})</span><br>
                </div>
                <div class="stat">
                    <span>AGI</span> <span>{{ Math.floor(waifu.Agi) }}</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.AGI)}})</span><br>
                </div>
                <div class="stat">
                    <span>DEX</span> <span>{{ Math.floor(waifu.Dex) }}</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.DEX)}})</span><br>
                </div>
                <div class="stat">
                    <span>LUCK</span> <span>{{ Math.floor(waifu.Luck) }}</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.LUCK)}})</span><br>
                </div>
                <br>
                <div class="stat">
                    <span>Physical</span> <span>{{ Math.floor(waifu.Physical) }}</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.Physical)}})</span><br>
                </div>
                <div class="stat">
                    <span>Psychic</span> <span>{{ Math.floor(waifu.Psychic) }}</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.Psychic)}})</span><br>
                </div>
                <div class="stat">
                    <span>Magical</span> <span>{{ Math.floor(waifu.Magical) }}</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.Magical)}})</span><br>
                </div>
                <div class="stat">
                    <span>Crit chance</span> <span>{{ Math.floor(waifu.CritChance*1000)/10 }}%</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.CritChance)}})</span><br>
                </div>
                <div class="stat">
                    <span>Crit damage</span> <span>{{ Math.floor(waifu.CritDamage*1000)/10 }}%</span>  <span class="modifier">({{waifu.DisplayModificator(StatModifier.CritDamage)}})</span><br>
                </div>
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
                <span v-if="count != undefined">Pull number {{ count+1 }}</span><br>
                STR : {{ waifu.b_str }}<br>
                KAW : {{ waifu.b_kaw }}<br>
                INT : {{ waifu.b_int }}<br>
                AGI : {{ waifu.b_agi }}<br>
                DEX : {{ waifu.b_dex }}<br>
                LUCK : {{ waifu.b_luck }}<br>
            </div>
        </div>
    </div>
</template>

<style lang="css" scoped>

.stat {
    display: grid;
    grid-template-columns: 2fr 1.5fr 2fr;
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
    top : 10px;
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
    height: 50vh;
    border-radius: 15px;
    display: grid;
    grid-template-columns: 1fr 1fr;
    padding: 2vh 2vh;
    z-index: 727;
    top:50%;
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

#waifuPic {
    border-radius: 15px;
    overflow: hidden;
}
#waifuInfos {
    padding: 0 1vw;
}

</style>