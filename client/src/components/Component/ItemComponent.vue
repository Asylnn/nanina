<script lang="ts">
import Equipment from '@/classes/item/equipment';
import Item from '@/classes/item/item';
import ItemType from '@/classes/item/item_type';
import ModifierComponent from './ModifierComponent.vue';
import GridDisplayComponent from './GridDisplayComponent.vue';
import Modifier from '@/classes/modifiers/modifiers';
import config from '../../../../baseValues.json'
import ClientResponseType from '@/classes/client_response_type';
import DisplayComponent from './DisplayComponent.vue';
import { Websocket, WebsocketEvent } from 'websocket-ts';
import ServerResponseType from '@/classes/server_response_type';
import type WebSocketResponse from '@/classes/web_socket_response';
import User from '@/classes/user/user';
import upgradeRequirementsByLevelByEquipmentType from '../../../../save/upgrade_requirements.json'
import itemDB from '../../../../save/item.json'
import EquipmentPiece from '@/classes/item/piece';
type UpgradeRequirement = {item_id: number, quantity:number}

export default {
    name : "ItemComponent",
    data() {
        return {
            ItemType : ItemType,
            showingUpgradePanel: false,
            publicPath : import.meta.env.BASE_URL,
            config:config, 
            listener: (() => {}) as (i: Websocket, ev: MessageEvent) => void,
            upgradeRequirements : [] as Array<UpgradeRequirement>,
            itemDB : itemDB,
        }
    },
    props: {
        isForEquiping: {
            type : Boolean,
            default:true,
            required : false,
        },
        isForLoot: {
            type : Boolean,
            default:false,
            required : false,
        },
        item: {
            type : [Item, Equipment],
            required : true
        },
        user:{
            type:User,
            required: false,
        },
    },
    components:{
        ModifierComponent,
        GridDisplayComponent,
        DisplayComponent,
    },
    methods:{
        upgrade()
        {
            if (this.user == undefined) return;
            this.SendToServer(ClientResponseType.UpgradeEquipment, this.item.inventoryId.toString(), this.user.Id)
        },
        lvlStarsCSS(upgrade : boolean = false)
        {
            return this.item.rarity == (this.item as Equipment).lvl + +upgrade - 2 ? "lvlMax" : ""
        },
        getQuantityRequiredStyle(req : UpgradeRequirement)
        {
            if((this.user && this.user.inventory.GetItem(req.item_id)?.count || 0) >= req.quantity)
                return '';
            else
                return 'color:red;'
        },
        updateUpgradeRequirements()
        {
            if(this.item.type != ItemType.Equipment) return;
            this.upgradeRequirements = (upgradeRequirementsByLevelByEquipmentType as any)[EquipmentPiece[(this.item as Equipment).piece].toLocaleLowerCase()][(this.item as Equipment).lvl - 1]
        }
    },
    computed:{
        allModifiers()
        {
            console.log(this.item)
            let modifiers = this.item.modifiers
            if(this.item.type == ItemType.Equipment) modifiers = modifiers.concat((this.item as Equipment).getAttributeModifiers())

            //Deepcopy of the poor!
            let u = Modifier.compactModifiers(JSON.parse(JSON.stringify(modifiers)))
            return u
        },
        upgradeQuantity()
        {
            return (this.item as Equipment).stat.amount*(config.equipment_main_stat_level_up_multiplicator - 1)
        },
        canUpgrade() : boolean
        {
            if (this.user == undefined) return false;
            return this.upgradeRequirements.every(requirement => (this.user!.inventory.GetItem(requirement.item_id)?.count || 0) >= requirement.quantity)
        }
    },
    mounted()
    {
        this.updateUpgradeRequirements()
        this.listener = (i: Websocket, ev: MessageEvent) => {
			var res : WebSocketResponse = JSON.parse(ev.data)
			switch (res.type) 
            {
                case ServerResponseType.ConfirmUpgrade:
                    if (this.user == undefined) break;
                    let equipment : Equipment = JSON.parse(res.data)
                    console.log("heyy")
                    console.log(equipment)
                    for(let requirement of this.upgradeRequirements)
                    {
                        this.user.inventory.RemoveItemWithId(requirement.item_id, requirement.quantity)
                    }
                    Object.assign<Equipment, Equipment>(this.item as Equipment, equipment)
                    this.updateUpgradeRequirements()
                    break
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
    <DisplayComponent :type="'item'">
        <div id="itemImageBox" @click="$emit('click')">
            <div><img id="itemImage" :src="`${publicPath}/item-image/${item.imgPATH}`"></div>
            <div v-if="item.type == ItemType.Equipment"  id="lvl" >
                <span v-if="!showingUpgradePanel" :class="lvlStarsCSS()">{{ "★".repeat((item as Equipment).lvl) }}</span>
                <span v-else >
                    <span>{{ "★".repeat((item as Equipment).lvl) }}</span> ➔ 
                    <span :class="lvlStarsCSS(true)">{{ "★".repeat((item as Equipment).lvl + 1) }}</span>
                </span>
            </div>
        </div>
        <div id="ItemInfo" @click="$emit('click')">
            {{ $t(`item.${item.id}.name`) }}<br>
            {{ $t(`item.${item.id}.description`) }}<br><br>
            <p v-if="item.type == ItemType.Equipment">
                {{ $t(`item.type.type`) }} : {{ $t(`item.type.${(item as Equipment).piece}`) }}<br>
                {{ $t(`set.set`) }} : {{ $t(`set.${(item as Equipment).setId}.name`) }}<br><br>
                {{ $t(`item.stat`) }}
                <ModifierComponent v-if="showingUpgradePanel" :modifier="(item as Equipment).stat" :upgrade-quantity="upgradeQuantity"></ModifierComponent>
                <ModifierComponent v-else :modifier="(item as Equipment).stat" ></ModifierComponent><br>
                
                <div v-if="(item as Equipment).attributes.length != 0 || showingUpgradePanel">
                    <p>{{$t("item.attributes.attribute")}} </p>
                    <div v-for="attribute in (item as Equipment).attributes">
                        <span>{{ $t(`item.attributes.${attribute.id}`) }}</span>
                    </div>
                    <p v-if="showingUpgradePanel" class="upgrade">{{$t("item.attributes.new")}} </p><br>
                </div>
                
            </p>
            <div v-if="allModifiers.length != 0">
                <p>{{$t("modifiers.modifier")}} </p>
                <div  v-for="modifier in allModifiers">
                    <ModifierComponent v-if="modifier != undefined" :modifier="modifier"></ModifierComponent>
                </div>
            </div><br>
            
            
            <div v-if="showingUpgradePanel"> <!--Upgrade Items-->
                <div  id="upgradeRequirements">
                    <div v-for="requirement in upgradeRequirements" class="upgradeRequirement flex">
                        <img :src="`${publicPath}/item-image/${itemDB[requirement.item_id as 0].imgPATH}`">
                        <span :style="getQuantityRequiredStyle(requirement)">{{ user!.inventory.GetItem(requirement.item_id)?.count || 0 }}/{{ requirement.quantity }}</span>
                    </div>
                </div>
                

                <button v-if="canUpgrade" class="smallbutton nnnbutton" @click="upgrade()">upgrade</button>
                
                
            </div><br>
            
            <span v-if="item.type == ItemType.Equipment && !isForLoot && item.rarity != (item as Equipment).lvl-2" class="clickable" @click="showingUpgradePanel = ! showingUpgradePanel">upgrade {{showingUpgradePanel ? "⤴" : "⤵"}}</span>
        </div>
    </DisplayComponent>
</template>

<style lang="css" scoped>

#upgradeRequirements
{
    display : flex;
    flex-direction: row;
    margin-bottom: 20px;
}
.upgradeRequirement
{
    padding-right: 20px;
    text-align: center;
}
.upgradeRequirement img
{
    width:64px;
    height:64px;
    
}

#itemImage {
    /*max-width: 25vw;
    max-height: 60vh;*/
    padding: 2.5vw 2.5vw 0.5vw 2.5vw;
    height: 196px;
    width: 196px;
}

#itemImageBox {
    display: flex;
    flex-direction: column;
    padding-right: 1vw;
}

#ItemInfo
{
    width:50vw
}

@media only screen and (orientation: landscape) {
    #ItemInfo
    {
        width:25vw
    }
}

.lvlMax
{
    color:gold;
}

#lvl {
    font-size:25px;
    text-align: center;
}

.isForEquiping {
    cursor:pointer;
}


</style>