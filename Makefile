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
	cat Compiler/input01.ibtl
	
	-$(RUNTIME) $(EXE) Compiler/input01.ibtl > stutest1.out
	cat stutest1.out

	cat Compiler/input02.ibtl
	-$(RUNTIME) $(EXE) Compiler/input02.ibtl > stutest2.out
	cat stutest2.out

proftest.out: compiler
	echo  $(PROFTEST)
	cat $(PROFTEST)
	
	$(RUNTIME) $(EXE) $(PROFTEST) > proftest.out
	cat proftest.out
