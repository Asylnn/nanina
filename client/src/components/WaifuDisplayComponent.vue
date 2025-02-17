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
        waifu: {
            type : Waifu,
            required : true
        },
        count: { //= -1 for single pull in gacha or for a display in WaifuListPage
            type : Number,
            required : true
        },
        user: {
            type : User,
            required : true
        }
    },
    mounted(){
        console.log(this.user.inventory.equipment)
    },
    methods:{

        openWeaponDisplay(equipment : Equipment){
            console.log("uwu")
            this.selected_item = equipment
            this.weaponVisible = true
        },
        closeWeaponDisplay()
        {
            this.selected_item = null
            this.weaponVisible = false
        },
        selectItem()
        {
            if(this.selected_item == null) return;
            switch(this.equipmentPiece){
                case EquipmentPiece.Weapon:
                    this.waifu.equipment.weapon = this.selected_item
                    break;
                case EquipmentPiece.Dress:
                    this.waifu.equipment.dress = this.selected_item
                    break;
                case EquipmentPiece.Accessory:
                    this.waifu.equipment.accessory = this.selected_item
                    break;
            }
            
            console.log('equiping')
            this.inventoryVisible = false
            this.SendToServer("equip item", JSON.stringify({equipmentId:this.selected_item.inventoryId, waifuId:this.waifu.id}), this.user.Id)
            this.closeWeaponDisplay()
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
        }
    },
    components:{
        GridDisplayComponent,
        ItemComponent
    },
    computed : {
        equipment_to_show() {
            return this.user.inventory.equipment.filter(equipment => equipment.piece == this.equipmentPiece)
        }
    }
}

</script>

<template>
    <div v-if="inventoryVisible" @click="inventoryVisible = false" class="veil" id="inventoryveil"></div>
    <GridDisplayComponent v-if="inventoryVisible" class="grid" @show-element="openWeaponDisplay" tabindex="0" @keydown.esc="" :elements="equipment_to_show" :columns=5></GridDisplayComponent>
    <div v-if="weaponVisible" @click="closeWeaponDisplay" class="veil" id="itemveil"></div>
    <ItemComponent @click="selectItem()" :is-for-equiping="true" v-if="selected_item != null" @input="" :item="selected_item" tabindex="0" @keydown.esc="inventoryVisible = false"></ItemComponent>

    <div id="focusedWaifu" >
        <div id="waifuPic"><img :src="`${publicPath}/waifu-image/${waifu.imgPATH}`"></div>
        <div id="waifuInfos">
            <span v-if="count != -1">Pull number {{ count+1 }}</span><br>
            {{waifu.name}} Level {{ waifu.lvl }} ({{ waifu.xp }} / {{ waifu.xpToLvlUp }})<br>
            <div v-if="!forPull">
                STR : {{ waifu.Str }}<br>
                KAW : {{ waifu.Kaw }}<br>
                INT : {{ waifu.Int }}<br>
                AGI : {{ waifu.Agi }}<br>
                DEX : {{ waifu.Dex }}<br>
                LUCK : {{ waifu.Luck }}<br>
                Physical : {{ waifu.Physical }} ({{waifu.DisplayModificator(StatModifier.Physical)}})<br>
                Psychic : {{ Math.floor(waifu.Psychic) }} ({{waifu.DisplayModificator(StatModifier.Psychic)}})<br>
                Magical : {{ waifu.Magical }} ({{waifu.DisplayModificator(StatModifier.Magical)}})<br>
                <div class="equipment">
                    <div @click="openDisplay(EquipmentPiece.Weapon)" class="itemSlot">
                        <img  :src="`${publicPath}item-image/${waifu.equipment.weapon?.imgPATH ?? 'unknown.svg'}`">
                    </div>
                    <div @click="openDisplay(EquipmentPiece.Dress)" class="itemSlot">
                        <img :src="`${publicPath}item-image/${waifu.equipment.dress?.imgPATH ?? 'unknown.svg'}`">
                    </div>
                    <div @click="openDisplay(EquipmentPiece.Accessory)" class="itemSlot">
                        <img :src="`${publicPath}item-image/${waifu.equipment.accessory?.imgPATH ?? 'unknown.svg'}`">
                    </div>
                </div>
            </div>
            
        </div>
    </div>
</template>

<style lang="css" scoped>

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

.grid {
    z-index: 780;
    position: sticky;
    top : 10px;
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
    top: 25vh; 
    left: 30vw;
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