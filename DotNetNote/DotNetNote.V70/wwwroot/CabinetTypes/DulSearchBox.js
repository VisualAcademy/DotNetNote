export default {
    props: {
        searchQueryChanged: Object 
    },
    data() {
        return {
            searchQuery: ""
        };
    },
    methods: {
        sendSearchQuery() {
            this.searchQueryChanged(this.searchQuery); // 부모로 검색어 전달 
        },
        searchHandler() {
            this.debounce(this.sendSearchQuery, 3000); 
        },
        debounce(fn, interval) {
            let debounceTimer
            return _ => {
                clearTimeout(debounceTimer)
                debounceTimer = setTimeout(_ => {
                    debounceTimer = null
                    fn.apply(this, arguments)
                }, interval)
            };
        },
        debounceSearch(event) {
            clearTimeout(this.debounce)
            this.debounce = setTimeout(() => {
                this.sendSearchQuery(); // 부모로 보내기 
            }, 300)
        }
    },
    created() {

    },
    template: `
<h3>검색</h3>
<div class="input-group mb-3">
    <input class="form-control form-control-sm form-control-borderless"
        type="search" placeholder="Search topics or keywords" aria-describedby="btnSearch"
        v-model="searchQuery"
        v-on:input="debounceSearch"
    />
    <div class="input-group-append">
        <button class="btn btn-sm btn-success" type="button"
        v-on:click="sendSearchQuery">Search</button>
    </div>
</div>
    `
}
