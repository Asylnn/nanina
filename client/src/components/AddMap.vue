<script lang="ts">

export default {
    name:"AddMap",
    data() {
        return {
            entered_id: 727
        }
    },
    props:{
        id : {
            type : String,
            required : true,
        }
    },
    methods : {
        updateSettings(){
            //@ts-ignore
			this.ws.send(JSON.stringify({type:"add beatmap with id", data:this.entered_id, id: this.id}))
        }
    }
}

</script>



<template>
    <div>
        <p>Enter Osu Beatmap Id to add it to the database</p>
        <span>Beatmap Id </span><input type="number" placeholder="727" v-model.number.lazy="entered_id">
        <button @click="updateSettings()">add</button>
    </div>
</template>