#--------------------------------------------
# INSTRUCTION
# Quoted strings are to be filled in by student
#
CCC = xbuild
CCFLAGS = 
OBJS = 
SOURCE = Compiler.sln
RUNFLAGS = 
EXE = Compiler/bin/Debug/Compiler.exe
RUNTIME = mono

#$(OBJS): $(SOURCE)
#	$(CCC) $(CCFLAGS) -c $(SOURCE)

compiler: 
	$(CCC) $(CCFLAGS)  $(SOURCE)

clean:
	rm -Rf Compiler/bin
	rm -Rf Compiler/obj
	ls Compiler

stutest.out: compiler
	cat Compiler/tests/final/input01.ibtl
	
	-$(RUNTIME) $(EXE) Compiler/tests/final/input01.ibtl > Compiler/out_0.gf
	cat Compiler/out_0.gf

	cat Compiler/tests/final/input02.ibtl
	-$(RUNTIME) $(EXE) Compiler/tests/final/input02.ibtl > Compiler/out_1.gf
	cat Compiler/out_1.gf

	cat Compiler/tests/final/input03.ibtl
	-$(RUNTIME) $(EXE) Compiler/tests/final/input02.ibtl >Compiler/out_2.gf
	cat Compiler/out_2.gf

	cat Compiler/tests/final/input04.ibtl
	-$(RUNTIME) $(EXE) Compiler/tests/final/input02.ibtl >Compiler/out_3.gf
	cat Compiler/out_3.gf

proftest.out: compiler
	echo  $(PROFTEST)
	cat $(PROFTEST)
	
	$(RUNTIME) $(EXE) $(PROFTEST) > proftest.out
	cat proftest.out
