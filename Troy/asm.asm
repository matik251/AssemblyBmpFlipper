.code 
GetValueFromASM proc
     mov rax, 2019
     ret
GetValueFromASM endp



MyProc1 PROC _array: QWORD, _array2: QWORD, _length: DWORD

mov rax, 8883848580818200h ;maska mniej znacz¹ca czêœæ
movq xmm15,rax
mov rax, 8C8D8E898A8B8687h ;maska bardziej znacz¹ca czêœæ
movq xmm0,rax
movlhps xmm15,xmm0 ;po³¹czenie masek

;r9 r10 wskaŸniki
mov r9,rcx ;_array - tablica Ÿród³owa
mov r10,rdx ;_array2 - tablica wynikowa

;r13 d³ugoœæ
mov r13,0 
mov r13d,r8d;_length

;r11 (d³ugoœæ-5) * 3
mov r11,r13
sub r11,5
mov rax,3
mul r11
mov r11,rax

add r9,r11	;r9 wskaŸnik czytania

;r14 licznik pêtli
mov r14,0
add r14,5

_fori: ;petla dla czytania po 5 pikseli- 15 bajtow

    cmp r14,r13	 ;i+5<szer
    jae _endfori
	
    movdqa xmm2,[r9]	;czyta ostatnie 5 pikseli
    
    vpshufb xmm1,xmm2,xmm15 ;zapis do xmm1 danych z xmm2 tak jak wskazuje maska w xmm15

	movdqa [r10],xmm1	;zapisuje 5 pikseli
	
    add r10,15	;przesuwa wskaŸnik zapisu o +15
    sub r9,15	;przesuwa wskaŸnik poboru o -15
	
	add r14,5	;przesuwa licznik o 5 pikseli
	jmp _fori
	
_endfori: ;petla dla czytania po pikselu- 1 bajt

	sub r14,5
	_forj:
	
		cmp r14,r13 
		jae _endforj
		
		movzx eax, byte ptr[r9]			; R
		movzx ebx, byte ptr[r9-1]		; G Czytanie piksela
		movzx ecx, byte ptr[r9-2]		; B
		
		mov byte ptr[r10],al			; R
		mov byte ptr[r10+1],bl			; G Zapis piksela
		mov byte ptr[r10+2],cl			; B
		
		add r10,3	;przesuwa wskaŸnik zapisu o +3
		sub r9,3	;przesuwa wskaŸnik poboru o -3
		
		inc r14		;inkrementuje licznik o 1 piksel
		
		jmp _forj
_endforj:

    ret
MyProc1 endp
end 

