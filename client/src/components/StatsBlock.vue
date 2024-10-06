<script lang="ts">

export default {
    name : "StatsBlock",
    props: {
        objectType : {
            type : String,
            required : true
        },
        stars : {
            type : Number,
            required : true
        },
        rarity : {
            type : Number,
            required : true
        },
        value : {
            type : Number,
            required : true
        },
        owner : {
            type : String,
            required : true
        },
        xp : {
            type : Number,
            required : true
        },
        lvl : {
            type : Number,
            required : true
        },
        b_int : {
            type : Number,
            required : true
        },
        b_luck : {
            type : Number,
            required : true
        },
        b_exp : {
            type : Number,
            required : true
        },
        o_int : {
            type : Number,
            required : true
        },
        o_luck : {
            type : Number,
            required : true
        },
        o_exp : {
            type : Number,
            required : true
        },
        u_int : {
            type : Number,
            required : true
        },
        u_exp : {
            type : Number,
            required : true
        },
        diffLvlup : {
            type : Number,
            required : true
        },
    }
}

</script>

<template>
    <div id="statsBlock">
        <div>
            <p style="margin-top: 50px">Je suis une {{ objectType }}
                <span v-for="n in stars">&#11088;</span> de rareté {{ rarity }}!</p>
            <p>Ma valeur est de {{ value }} car je suis unique ! &#128525;</p>
            <p>J'ai été recueillie par mon amour, {{ owner }} ! &#128536;</p>
            <p>J'ai actuellement {{ xp }} xp et au level {{ lvl }} &#128546;</p>
            <p>Voici mes stats actuelles :</p>
            <p>{{ b_int }} {{ b_luck }} {{ b_exp }} &#127800;</p>
            <p>J'ai démarré avec ces stats là :</p>
            <p>{{ o_int }} {{ o_luck }} {{ o_exp }} &#127800;</p>
            <p>Si tu joues avec moi je peux gagner ces stats là ! &#128150;</p>
            <p>{{ u_int }} {{ u_exp }} &#127800;</p>
            <p>Mais je ne suis pas une fille facile ! J'ai du mal à lvl up avec ma difficulté de {{ diffLvlup }} ! &#128533;</p>
        </div>
    </div>
</template>

<style lang="css" scoped>
#statsBlock {
    width: 60%;
    float: left;
    border: 2px solid orange;
}  
</style>