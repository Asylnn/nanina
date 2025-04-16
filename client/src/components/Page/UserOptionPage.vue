<script lang="ts">

import Game from '@/classes/user/game';
import User from '@/classes/user/user';


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
            selected_prefered_game: this.user.preferedGame,
            entered_id :this.user.ids.osuId,
            request : false,
            code: 0,
            entered_token: "",
            Game:Game
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
        onChangePreferedGame(){
            this.user.preferedGame = this.selected_prefered_game
            this.SendToServer("update prefered game", (+this.selected_prefered_game).toString(), this.user.Id)
        },
        updateSettings(){
            this.request = true
            this.SendToServer("update osu id", this.entered_id.toString(), this.user.Id)
        },
        verifyOsuId(){
            this.SendToServer("verify osu id", this.code.toString(), this.user.Id)
        },
        verifyMaimaiToken(){
            this.SendToServer("verify maimai token", this.entered_token.toString(), this.user.Id)
        },
        async Disconect(){
            this.SendToServer("disconect", "", this.user.Id)
            await new Promise(r => setTimeout(r, 200));
            location.href = "/"
        }
    }
}

</script>



<template>
    <div class="grid" id="optionsGrid">
        <div id="optionTheme">
            <p>{{ $t("option.change_theme") }}</p>
            <select v-model="selected_theme" @change="onChangeTheme()">
                <option value = "dark_theme">{{ $t("option.dark_theme") }}</option>
                <option value = "white_theme">{{ $t("option.white_theme") }}</option>
                <option value = "cute_theme">{{ $t("option.cute_theme") }}</option>
            </select>
        </div>
        <div class="grid" id="optionGame">
            <p>{{ $t("option.prefered_game") }}</p>
            <select v-model="selected_prefered_game" @change="onChangePreferedGame()">
                <option :value="Game.OsuStandard" >{{ $t("games.osu_standard") }}</option>
                <option :value="Game.MaimaiFinale">{{ $t("games.maimai_finale") }}</option>
            </select>
        </div>
        <div id="optionOsuId">
            <p>{{ $t('option.link_osu_id')}}</p>
            <span>
                <input type="number" v-model.number.lazy="entered_id">
                <span id="userOsuId" v-if="user.verification.isOsuIdVerified"><img src="../assets/green_checkmark.png"></span>
            </span>
            <button @click="updateSettings()">{{ $t('option.update') }}</button>
        </div>
        <div id="optionVerifOsuId" v-if=request>
            <p>{{ $t('option.got_code') }}</p>
            <input type="number" v-model.number.lazy="code">
            <button @click="verifyOsuId()">{{ $t('option.verify_code') }}</button>
        </div>
        
        <p>{{ $t('option.link_maimai_token') }}</p>
        <div class="grid" id="maimaitoken">
            <input type="text" v-model.lazy="entered_token">
            <span  v-if="user.verification.isMaimaiTokenVerified"><img src="../assets/green_checkmark.png"></span>
            <button @click="verifyMaimaiToken()">{{ $t('option.update') }}</button>
        </div>
        
        <button @click="Disconect()">{{ $t('option.disconnect') }}</button>
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

#optionGame, #optionTheme {
    grid-template-columns: 1fr 1fr; 
}

#optionOsuId {
    grid-template-columns: 2fr 0.6fr 1.4fr;   
}

#maimaitoken {
    grid-template-columns: 0fr 0.2fr 2fr;   
}

#optionVerifOsuId {
    grid-template-columns: 1.3fr 0.25fr 0.45fr;
}

#optionTheme select, #optionOsuId button, #optionOsuId input {
    border-radius: 5px;
}

#optionTheme select, #optionOsuId button {
    cursor: pointer;
}

#optionTheme select, #optionGame select {
    width: 30%;
}

#optionOsuId button {
    width: 20%;
}

button {
    width: 10%;
}

#userOsuId {
    color: rgb(0, 211, 0);
}

#optionOsuId input {
    width: 5.8vw;
}

#maimaitoken input {
    width: 20vw;
}

#userOsuId img, #maimaitoken img {
    padding-left: 0.3vw;
    align-self: center;
    width: 1.25vw;
}

#optionVerifOsuId input{
    width: 5.8vw;
}

#optionVerifOsuId button {
    width: 5.8vw;
}
</style>