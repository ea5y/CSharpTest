"Set vundle
set nocompatible
filetype on

set rtp+=~/.vim/bundle/Vundle.vim
call vundle#begin()

"my bundle
Plugin 'VundleVim/Vundle.vim'
Plugin 'tpope/vim-fugitive'
Plugin 'vim-airline/vim-airline'
Plugin 'vim-airline/vim-airline-themes'
Plugin 'eugeii/consolas-powerline-vim'
Plugin 'scrooloose/nerdtree'

Plugin 'Valloric/YouCompleteMe'

Plugin 'OmniSharp/omnisharp-vim'
Plugin 'tpope/vim-dispatch'
Plugin 'vim-syntastic/syntastic'
"Plugin 'ctrlpvim/ctrlp.vim'
Plugin 'SirVer/ultisnips'
"Plugin 'ervandew/supertab'

"Plugin 'brookhong/cscope.vim'

"color schemes
Plugin 'tomasr/molokai'
Plugin 'Lucius'
Plugin 'altercation/solarized'
Plugin 'NLKNguyen/papercolor-theme'
Plugin 'flazz/vim-colorschemes'

"Plugin 'suan/vim-instant-markdown'
Plugin 'iamcco/markdown-preview.vim'
call vundle#end()
filetype plugin indent on
filetype plugin on
"---------------------------------------
"------Plugin: Airline
"---------------------------------------
"font theme
let g:airline_theme="luna"
"enable tabline
let g:airline#extensions#tabline#enabled = 1
let g:airline#extensions#tabline#buffer_nr_show = 1

let g:airline_powerline_fonts = 1
let g:Powerline_symbols = 'fancy'
"let g:Powerline_symbols = 'molokai'
set guifont=Inconsolata\ for\ Powerline
"set guifont=arial\ monospaced\ for\ sap
set encoding=utf-8
set langmenu=zh_CN.UTF-8
set fileencodings=ucs-bom,utf-8,cp936,gb18030,big5,euc-jp,euc-kr,latin1
set ambiwidth=double
set laststatus=2

"buffer map
nnoremap <s-k> :bn<cr>
nnoremap <s-j> :bp<cr>
let g:airline#extensions#whitespace#enabled = 0
let g:airline#extensions#whitespace#symbol = '!'

if !exists('g:airline_symbols')
	let g:airline_symbols = {}
endif
" old vim-powerline symbols
let g:airline_left_sep = '⮀'
let g:airline_left_alt_sep = '⮁'
let g:airline_right_sep = '⮂'
let g:airline_right_alt_sep = '⮃'
let g:airline_symbols.branch = '⭠'
let g:airline_symbols.readonly = '⭤'
let g:airline_symbols.linenr = '⭡'

"---------------------------------------
"------Vim Enter
"---------------------------------------
function s:ReadSession()
	let s:session_file = 'Session.vim'
	if filereadable(s:session_file)
		source Session.vim  
	endif
endfunction
function s:ReadViminfo()
	let s:viminfo_file = 'Viminfo.viminfo'
	if filereadable(s:viminfo_file)
		rviminfo Viminfo.viminfo 
	endif
endfunction
"autocmd VimEnter * :call s:ReadSession()
"autocmd VimEnter * :call s:ReadViminfo()

map <leader>nt :NERDTreeToggle<cr>
map <leader>pj :source Session.vim<cr>

"---------------------------------------
"------Vim Exit
"---------------------------------------
set sessionoptions=buffers,sesdir,folds,help,options,tabpages,winsize
"autocmd VimLeave * mks! ./Session.vim
"autocmd VimLeave * wviminfo! Viminfo.viminfo
map <leader>ws :mks! Session.vim<cr>

"-------------------------------------------
"------Plugin Ycm
"-------------------------------------------
let g:ycm_autoclose_preview_window_after_insertion = 1
let g:ycm_warning_symbol = '>'
let g:ycm_enable_diagnostic_signs = 0
set completeopt-=preview

"-------------------------------------------
"------Plugin OmniSharp
"-------------------------------------------
filetype plugin on
"set splitbelow
"let g:syntastic_cs_checkers = ['syntax', 'semantic', 'issues']
let g:OmniSharp_selector_ui = 'ctrlp'


augroup omnisharp_commands
    autocmd!

    "Set autocomplete function to OmniSharp (if not using YouCompleteMe completion plugin)
    autocmd FileType cs setlocal omnifunc=OmniSharp#Complete

    " Synchronous build (blocks Vim)
    "autocmd FileType cs nnoremap <F5> :wa!<cr>:OmniSharpBuild<cr>
    " Builds can also run asynchronously with vim-dispatch installed
    autocmd FileType cs nnoremap <leader>b :wa!<cr>:OmniSharpBuildAsync<cr>
    " automatic syntax check on events (TextChanged requires Vim 7.4)
    autocmd BufEnter,TextChanged,InsertLeave *.cs SyntasticCheck

    " Automatically add new cs files to the nearest project on save
    autocmd BufWritePost *.cs call OmniSharp#AddToProject()

    "show type information automatically when the cursor stops moving
    autocmd CursorHold *.cs call OmniSharp#TypeLookupWithoutDocumentation()

    "The following commands are contextual, based on the current cursor position.

    autocmd FileType cs nnoremap gd :OmniSharpGotoDefinition<cr>
    autocmd FileType cs nnoremap <leader>fi :OmniSharpFindImplementations<cr>
    autocmd FileType cs nnoremap <leader>ft :OmniSharpFindType<cr>
    autocmd FileType cs nnoremap <leader>fs :OmniSharpFindSymbol<cr>
    autocmd FileType cs nnoremap <leader>fu :OmniSharpFindUsages<cr>
    "finds members in the current buffer
    autocmd FileType cs nnoremap <leader>fm :OmniSharpFindMembers<cr>
    " cursor can be anywhere on the line containing an issue
    autocmd FileType cs nnoremap <leader>x  :OmniSharpFixIssue<cr>
    autocmd FileType cs nnoremap <leader>fx :OmniSharpFixUsings<cr>
    autocmd FileType cs nnoremap <leader>tt :OmniSharpTypeLookup<cr>
    autocmd FileType cs nnoremap <leader>dc :OmniSharpDocumentation<cr>
    "navigate up by method/property/field
    autocmd FileType cs nnoremap <C-K> :OmniSharpNavigateUp<cr>
    "navigate down by method/property/field
    autocmd FileType cs nnoremap <C-J> :OmniSharpNavigateDown<cr>

augroup END

" this setting controls how long to wait (in ms) before fetching type / symbol information.
set updatetime=500
" Remove 'Press Enter to continue' message when type information is longer than one line.
set cmdheight=2

" Contextual code actions (requires CtrlP or unite.vim)
nnoremap <leader><space> :OmniSharpGetCodeActions<cr>
" Run code actions with text selected in visual mode to extract method
vnoremap <leader><space> :call OmniSharp#GetCodeActions('visual')<cr>

" rename with dialog
nnoremap <leader>nm :OmniSharpRename<cr>
nnoremap <F2> :OmniSharpRename<cr>
" rename without dialog - with cursor on the symbol to rename... ':Rename newname'
command! -nargs=1 Rename :call OmniSharp#RenameTo("<args>")

" Force OmniSharp to reload the solution. Useful when switching branches etc.
nnoremap <leader>rl :OmniSharpReloadSolution<cr>
nnoremap <leader>cf :OmniSharpCodeFormat<cr>
" Load the current .cs file to the nearest project
nnoremap <leader>tp :OmniSharpAddToProject<cr>

" (Experimental - uses vim-dispatch or vimproc plugin) - Start the omnisharp server for the current solution
nnoremap <leader>ss :OmniSharpStartServer<cr>
nnoremap <leader>sp :OmniSharpStopServer<cr>

" Add syntax highlighting for types and interfaces
nnoremap <leader>th :OmniSharpHighlightTypes<cr>
"Don't ask to save when changing buffers (i.e. when jumping to a type definition)
set hidden

" Enable snippet completion, requires completeopt-=preview
let g:OmniSharp_want_snippet=1

"-------------------------------------------
"------Plugin ultisnips
"-------------------------------------------
" Trigger configuration. Do not use <tab> if you use https://github.com/Valloric/YouCompleteMe.
let g:UltiSnipsExpandTrigger="<tab>"
let g:UltiSnipsJumpForwardTrigger="<c-b>"
let g:UltiSnipsJumpBackwardTrigger="<c-z>"

" If you want :UltiSnipsEdit to split your window.
let g:UltiSnipsEditSplit="vertical"

"===============Not Plugin===================
"Set template
"Autocmd BufNewFile *.txt or ~/.vim/template/txt.tlp
"Let g:Timestamp = .strftime("%Y-%m-%d %H:%M")
"sh
function LoadShellScriptTemplate()
	call append(0,"#!/bin/bash")
	call append(1,"#Program:")
	call append(2,"#======>")
	call append(3,"#Author: easy")
	call append(4,"#Data: ".strftime("%Y-%m-%d %H:%M"))
	call append(5,"#Update: ".strftime("%Y-%m-%d %H:%M"))
	call append(6,"PATH=/bin:/sbin:/usr/bin:/usr/sbin:/usr/local/bin:/usr/local/sbin:~/bin")
	call append(7,"export PATH")
	call append(8,"#===========")
endf
autocmd BufNewFile *.sh call LoadShellScriptTemplate()

"cs
function LoadCSharpTemplate()
	call append(0,"//================================")
	call append(1,"//===Author: easy")
	call append(2,"//===Email: gopiny@live.com")
	call append(3,"//===Date: ".strftime("%Y-%m-%d %H:%M"))
	call append(4,"//================================")
endf
autocmd BufNewFile *.cs call LoadCSharpTemplate()
function UpdataCSharpTemplate()
	call append(4,"//===Update: ".strftime("%Y-%m-%d %H:%M"))
endf
"autocmd BufRead *.cs call UpdataCSharpTemplate()

"txt
function InputTimestamp()
	call append(0,"Timestamp: ".strftime("%Y-%m-%d %H:%M"))
endf
autocmd BufNewFile *.txt call InputTimestamp()

"Set map
function SetMap_F5()
	if &filetype == 'text'
		call InputTimestamp()
	endif
endf
noremap <f5> :call SetMap_F5() <CR>

"Set fullscreen
let g:fullscreen = 0
function! ToggleFullscreen()
	if g:fullscreen == 1
		let g:fullscreen = 0
		let mod = "remove"
	else		
		let g:fullscreen = 1
		let mod = "add"
	endif
	call system("wmctrl -ir" . v:windowid . " -b " . mod . ",fullscreen")
endfunction

map <silent> <F11> :call ToggleFullscreen()<CR>

set softtabstop=4
set shiftwidth=4
set autoindent

"Hide menu
set guioptions-=m
"Hide toolbar
set guioptions-=T
"Hide left scroll bar
set guioptions-=L
"Hide right scroll bar
set guioptions-=r
"Hide bottom scroll bar
set guioptions-=b

"Set colorscheme
colorscheme molokai
"colorscheme solarized
"colorscheme elflord
"colorscheme Lucius
"Set tab length
set tabstop=4
"Set line number
set number
"Set copy paste
nmap ,v "+p
vmap ,c "+yy
nmap ,c "+yy

highlight YcmWarningSection guifg=#808080
highlight Comment ctermfg=green guifg=green
