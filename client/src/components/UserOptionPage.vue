<script lang="ts">

import User from '@/classes/user';


/*
Theme
Add osu id

Notifs : checkbox avec toutes les categ
si desactive checkbox, n'envoie pas les notif de quelle categ


Gacha : afficher le montant pour pull, mettre texte en rouge et disable le bouton si pas assez de ressources
Claim : Timer de 1min pour fight / Timer de 3min pour claim #Si pas de score trouvé, mettre timer et afficher un texte pour inciter a jouer la map










*/ 


export default {
    name:"UserOptionPage",
    data() {
        return {
            selected_theme :this.theme,
            entered_id :this.user.ids.osuId,
            request : false,
        }
    },
    props:{
        theme : {
            type : String, 
            required : true,
        },
        user : {
            type : User,
            required : true,
        }
    },
    emits : ["theme-change"],
    methods : {
        onChangeTheme(){
            this.$emit("theme-change", this.selected_theme)
        },
        updateSettings(){
            //@ts-ignore
            this.request = true
			this.ws.send(JSON.stringify({type:"update osu id", data:this.entered_id, id: this.user.Id}))
        },
    }
}

</script>



<template>
    <div class="grid" id="optionsGrid">
        <div id="optionTheme">
            <p>Choose your favourite theme !</p>
            <select v-model="selected_theme" @change="onChangeTheme()">
                <option value = "dark_theme">Dark Theme</option>
                <option value = "white_theme">White Theme</option>
                <option value = "cute_theme">Cute Theme</option>
            </select>
        </div>
        <div id="optionOsuId">
            <p>Link us your osu id so we can steal your pp !</p>
            <input type="number" v-model.number.lazy="entered_id">
            <span id="userOsuId" v-if="user.ids.osuId != '0'"><img src="../assets/green_checkmark.png"></span>
            <button @click="updateSettings()">Update</button>
        </div>
        <div id="optionVerifOsuId" v-if=request>
            <p>You just recieved a unique code in your osu! dms, enter it here to update your id =></p>
            <input type="number">
        </div>
    </div>
</template>

<style lang="css" scoped>

.grid, #optionOsuId, #optionTheme, #optionVerifOsuId {
    display: grid;
}
#optionsGrid {
    padding: 0 20vw;
}
#optionTheme, #optionOsuId, #optionVerifOsuId {
    padding-top: 1vh;
}
#optionTheme {
    grid-template-columns: 1fr 1fr; 
}
#optionOsuId {
    grid-template-columns: 2fr 0.3fr 0.2fr 1.4fr;   
}
#optionVerifOsuId {
    grid-template-columns: 1.3fr 0.7fr;
}
#optionTheme select, #optionOsuId button, #optionOsuId input {
    border-radius: 5px;
}
#optionTheme select, #optionOsuId button {
    cursor: pointer;
}
#optionTheme select {
    width: 20%;
}
#optionOsuId button {
    width: 20%;
}
#userOsuId {
    color: rgb(0, 211, 0);
}
#optionOsuId input {
    width: 5.8vw;
}
#userOsuId img {
    padding-left: 0.3vw;
    align-self: center;
    width: 1.25vw;
}
#optionVerifOsuId input{
    width: 5.8vw;
}

</style>